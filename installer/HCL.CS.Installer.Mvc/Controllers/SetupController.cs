/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Mvc;
using HclCsInstallerMVC.Application.Exceptions;
using HclCsInstallerMVC.Models;
using HclCsInstallerMVC.Services;
using HclCsInstallerMVC.ViewModels;

namespace HclCsInstallerMVC.Controllers;

[Route("setup")]
public sealed class SetupController : Controller
{
    private readonly IInstallerWorkflowService _workflowService;

    public SetupController(IInstallerWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Root(CancellationToken cancellationToken)
    {
        if (await _workflowService.IsInstallationCompletedAsync(cancellationToken))
            return RedirectToAction(nameof(Installed));

        return RedirectToAction(nameof(Provider));
    }

    [HttpGet("")]
    public async Task<IActionResult> Provider(CancellationToken cancellationToken)
    {
        if (await _workflowService.IsInstallationCompletedAsync(cancellationToken))
            return RedirectToAction(nameof(Installed));

        ViewData["CurrentStep"] = 1;
        var model = await _workflowService.GetProviderSelectionAsync(cancellationToken);
        return View(model);
    }

    [HttpPost("provider")]
    public async Task<IActionResult> SaveProvider(
        [Bind(nameof(SetupProviderViewModel.Provider))] SetupProviderViewModel model,
        CancellationToken cancellationToken)
    {
        if (await _workflowService.IsInstallationCompletedAsync(cancellationToken))
            return RedirectToAction(nameof(Installed));

        ViewData["CurrentStep"] = 1;
        if (!ModelState.IsValid) return View("Provider", model);

        try
        {
            await _workflowService.SaveProviderSelectionAsync(model, cancellationToken);
            return RedirectToAction(nameof(Connection));
        }
        catch (InstallationAlreadyCompletedException)
        {
            return RedirectToAction(nameof(Installed));
        }
    }

    [HttpGet("connection")]
    public async Task<IActionResult> Connection(CancellationToken cancellationToken)
    {
        if (await _workflowService.IsInstallationCompletedAsync(cancellationToken))
            return RedirectToAction(nameof(Installed));

        ViewData["CurrentStep"] = 2;
        var model = await _workflowService.GetConnectionConfigurationAsync(cancellationToken);
        if (model.Provider is null) return RedirectToAction(nameof(Provider));

        return View(model);
    }

    [HttpPost("connection")]
    public async Task<IActionResult> SaveConnection(
        [Bind(nameof(SetupConnectionViewModel.Provider), nameof(SetupConnectionViewModel.ConnectionString))]
        SetupConnectionViewModel model,
        CancellationToken cancellationToken)
    {
        if (await _workflowService.IsInstallationCompletedAsync(cancellationToken))
            return RedirectToAction(nameof(Installed));

        ViewData["CurrentStep"] = 2;
        if (!ModelState.IsValid) return View("Connection", model);

        try
        {
            await _workflowService.SaveConnectionConfigurationAsync(model, cancellationToken);
            return RedirectToAction(nameof(Validate));
        }
        catch (InstallationAlreadyCompletedException)
        {
            return RedirectToAction(nameof(Installed));
        }
    }

    [HttpGet("validate")]
    public async Task<IActionResult> Validate(CancellationToken cancellationToken)
    {
        if (await _workflowService.IsInstallationCompletedAsync(cancellationToken))
            return RedirectToAction(nameof(Installed));

        ViewData["CurrentStep"] = 3;
        var model = await _workflowService.GetConnectionValidationAsync(cancellationToken);
        if (!model.HasConfiguration || string.IsNullOrWhiteSpace(model.ConnectionString))
            return RedirectToAction(nameof(Connection));

        return View(model);
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ExecuteValidation(CancellationToken cancellationToken)
    {
        if (await _workflowService.IsInstallationCompletedAsync(cancellationToken))
            return RedirectToAction(nameof(Installed));

        ViewData["CurrentStep"] = 3;

        try
        {
            var model = await _workflowService.ValidateConnectionAsync(cancellationToken);
            if (model.IsSuccessful) return RedirectToAction(nameof(Migrate));

            return View("Validate", model);
        }
        catch (InstallationAlreadyCompletedException)
        {
            return RedirectToAction(nameof(Installed));
        }
    }

    [HttpGet("migrate")]
    public async Task<IActionResult> Migrate(CancellationToken cancellationToken)
    {
        if (await _workflowService.IsInstallationCompletedAsync(cancellationToken))
            return RedirectToAction(nameof(Installed));

        ViewData["CurrentStep"] = 4;
        var model = await _workflowService.GetMigrationViewModelAsync(cancellationToken);
        if (!model.CanRun) return RedirectToAction(nameof(Validate));

        return View(model);
    }

    [HttpPost("migrate")]
    public async Task<IActionResult> RunMigrations(CancellationToken cancellationToken)
    {
        if (await _workflowService.IsInstallationCompletedAsync(cancellationToken))
            return RedirectToAction(nameof(Installed));

        ViewData["CurrentStep"] = 4;

        try
        {
            var model = await _workflowService.RunMigrationAsync(cancellationToken);
            if (model.IsCompleted) return RedirectToAction(nameof(Seed));

            return View("Migrate", model);
        }
        catch (InstallerWorkflowException workflowException)
        {
            ModelState.AddModelError(string.Empty, workflowException.Message);
            var model = await _workflowService.GetMigrationViewModelAsync(cancellationToken);
            return View("Migrate", model);
        }
        catch (InstallationAlreadyCompletedException)
        {
            return RedirectToAction(nameof(Installed));
        }
    }

    [HttpGet("seed")]
    public async Task<IActionResult> Seed(CancellationToken cancellationToken)
    {
        if (await _workflowService.IsInstallationCompletedAsync(cancellationToken))
            return RedirectToAction(nameof(Installed));

        ViewData["CurrentStep"] = 5;
        var migrationState = await _workflowService.GetMigrationViewModelAsync(cancellationToken);
        if (!migrationState.IsCompleted) return RedirectToAction(nameof(Migrate));

        var model = await _workflowService.GetSeedViewModelAsync(cancellationToken);
        return View(model);
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed(
        [Bind(
            nameof(SeedStepViewModel.ClientName),
            nameof(SeedStepViewModel.ClientUri),
            nameof(SeedStepViewModel.UseAuthorizationCodeGrant),
            nameof(SeedStepViewModel.UseClientCredentialsGrant),
            nameof(SeedStepViewModel.UseRefreshTokenGrant),
            nameof(SeedStepViewModel.UsePasswordGrant),
            nameof(SeedStepViewModel.UseCodeResponseType),
            nameof(SeedStepViewModel.UseDefaultScopes),
            nameof(SeedStepViewModel.AllowedScopes),
            nameof(SeedStepViewModel.RedirectUris),
            nameof(SeedStepViewModel.PostLogoutRedirectUris),
            nameof(SeedStepViewModel.FrontChannelLogoutUri),
            nameof(SeedStepViewModel.BackChannelLogoutUri),
            nameof(SeedStepViewModel.UserName),
            nameof(SeedStepViewModel.Password),
            nameof(SeedStepViewModel.ConfirmPassword),
            nameof(SeedStepViewModel.FirstName),
            nameof(SeedStepViewModel.LastName),
            nameof(SeedStepViewModel.Email),
            nameof(SeedStepViewModel.PhoneNumber),
            nameof(SeedStepViewModel.IdentityProvider))]
        SeedStepViewModel model,
        CancellationToken cancellationToken)
    {
        if (await _workflowService.IsInstallationCompletedAsync(cancellationToken))
            return RedirectToAction(nameof(Installed));

        ViewData["CurrentStep"] = 5;
        if (!ModelState.IsValid) return View(model);

        try
        {
            var result = await _workflowService.ExecuteSeedAsync(model, cancellationToken);
            if (result.IsCompleted) return RedirectToAction(nameof(Complete));

            return View(result);
        }
        catch (InstallerWorkflowException workflowException)
        {
            ModelState.AddModelError(string.Empty, workflowException.Message);
            return View(model);
        }
        catch (InstallationAlreadyCompletedException)
        {
            return RedirectToAction(nameof(Installed));
        }
    }

    [HttpPost("skip-seed")]
    public async Task<IActionResult> SkipSeed(CancellationToken cancellationToken)
    {
        if (await _workflowService.IsInstallationCompletedAsync(cancellationToken))
            return RedirectToAction(nameof(Installed));

        try
        {
            await _workflowService.SkipSeedAsync(cancellationToken);
            return RedirectToAction(nameof(Complete));
        }
        catch (InstallationAlreadyCompletedException)
        {
            return RedirectToAction(nameof(Installed));
        }
    }

    [HttpGet("/complete")]
    public async Task<IActionResult> Complete(CancellationToken cancellationToken)
    {
        ViewData["CurrentStep"] = 6;
        var model = await _workflowService.GetCompletionViewModelAsync(cancellationToken);
        return View(model);
    }

    [HttpGet("/installed")]
    public async Task<IActionResult> Installed(CancellationToken cancellationToken)
    {
        ViewData["CurrentStep"] = 6;
        var model = await _workflowService.GetCompletionViewModelAsync(cancellationToken);
        model.AlreadyInstalled = true;
        model.Message = "Setup is already completed for this environment.";
        return View("Complete", model);
    }

    [HttpGet("/error")]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }
}
