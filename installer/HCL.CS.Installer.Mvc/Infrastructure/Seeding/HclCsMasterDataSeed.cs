/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.Enums;
using static HCL.CS.Domain.Constants.Endpoint.OpenIdConstants;

namespace HclCsInstallerMVC.Infrastructure.Seeding;

public static class HclCsMasterDataSeed
{
    public static List<ApiResources> GetApiResourceEntityMaster()
    {
        return
        [
            new ApiResources
            {
                Name = "hcl-cs.apiresource",
                DisplayName = "HCL.CS.APIRESOURCE",
                Description = "To register and manage ApiResource",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "HclCsUser",
                ApiScopes =
                [
                    new()
                    {
                        Name = "hcl-cs.apiresource.manage", DisplayName = "HCL.CS.APIRESOURCE.MANAGE",
                        Description = "Manage api resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },

                    new()
                    {
                        Name = "hcl-cs.apiresource.read", DisplayName = "HCL.CS.APIRESOURCE.READ",
                        Description = "Read api resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },

                    new()
                    {
                        Name = "hcl-cs.apiresource.write", DisplayName = "HCL.CS.APIRESOURCE.WRITE",
                        Description = "Write api resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },

                    new()
                    {
                        Name = "hcl-cs.apiresource.delete", DisplayName = "HCL.CS.APIRESOURCE.DELETE",
                        Description = "Delete api resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                ]
            },

            new()
            {
                Name = "hcl-cs.identityresource",
                DisplayName = "HCL.CS.IDENTITYRESOURCE",
                Description = "To register and manage IdentityResource",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "HclCsUser",
                ApiScopes = new List<ApiScopes>
                {
                    new()
                    {
                        Name = "hcl-cs.identityresource.manage", DisplayName = "HCL.CS.IDENTITYRESOURCE.MANAGE",
                        Description = "Manage identityresource resource", CreatedBy = "HclCsUser",
                        CreatedOn = DateTime.UtcNow, IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.identityresource.read", DisplayName = "HCL.CS.IDENTITYRESOURCE.READ",
                        Description = "Read identityresource resource", CreatedBy = "HclCsUser",
                        CreatedOn = DateTime.UtcNow, IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.identityresource.write", DisplayName = "HCL.CS.IDENTITYRESOURCE.WRITE",
                        Description = "Write identityresource resource", CreatedBy = "HclCsUser",
                        CreatedOn = DateTime.UtcNow, IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.identityresource.delete", DisplayName = "HCL.CS.IDENTITYRESOURCE.DELETE",
                        Description = "Delete identityresource resource", CreatedBy = "HclCsUser",
                        CreatedOn = DateTime.UtcNow, IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                }
            },

            new()
            {
                Name = "hcl-cs.client",
                DisplayName = "HCL.CS.CLIENT",
                Description = "To register and manage client",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "HclCsUser",
                ApiScopes = new List<ApiScopes>
                {
                    new()
                    {
                        Name = "hcl-cs.client.manage", DisplayName = "HCL.CS.CLIENT.MANAGE",
                        Description = "Manage client resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.client.read", DisplayName = "HCL.CS.CLIENT.READ",
                        Description = "Read client resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.client.write", DisplayName = "HCL.CS.CLIENT.WRITE",
                        Description = "Write client resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.client.delete", DisplayName = "HCL.CS.CLIENT.DELETE",
                        Description = "Delete client resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                }
            },

            new()
            {
                Name = "hcl-cs.role",
                DisplayName = "HCL.CS.ROLE",
                Description = "To register and manage role",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "HclCsUser",
                ApiScopes = new List<ApiScopes>
                {
                    new()
                    {
                        Name = "hcl-cs.role.manage", DisplayName = "HCL.CS.ROLE.MANAGE",
                        Description = "Manage role resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.role.read", DisplayName = "HCL.CS.ROLE.READ", Description = "Read role resource",
                        CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.role.write", DisplayName = "HCL.CS.ROLE.WRITE",
                        Description = "Write role resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.role.delete", DisplayName = "HCL.CS.ROLE.DELETE",
                        Description = "Delete role resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                }
            },

            new()
            {
                Name = "hcl-cs.adminuser",
                DisplayName = "HCL.CS.ADMINUSER",
                Description = "To register and manage adminuser",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "HclCsUser",
                ApiScopes = new List<ApiScopes>
                {
                    new()
                    {
                        Name = "hcl-cs.adminuser.manage", DisplayName = "HCL.CS.ADMINUSER.MANAGE",
                        Description = "Manage admin resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.adminuser.read", DisplayName = "HCL.CS.ADMINUSER.READ",
                        Description = "Read admin resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.adminuser.write", DisplayName = "HCL.CS.ADMINUSER.WRITE",
                        Description = "Write admin resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.adminuser.delete", DisplayName = "HCL.CS.ADMINUSER.DELETE",
                        Description = "Delete admin resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                }
            },

            new()
            {
                Name = "hcl-cs.user",
                DisplayName = "HCL.CS.USER",
                Description = "To register and manage user",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "HclCsUser",
                ApiScopes = new List<ApiScopes>
                {
                    new()
                    {
                        Name = "hcl-cs.user.manage", DisplayName = "HCL.CS.USER.MANAGE",
                        Description = "Manage admin resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.user.read", DisplayName = "HCL.CS.USER.READ",
                        Description = "Read admin resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.user.write", DisplayName = "HCL.CS.USER.WRITE",
                        Description = "Write admin resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.user.delete", DisplayName = "HCL.CS.USER.DELETE",
                        Description = "Delete admin resource", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                }
            },

            new()
            {
                Name = "hcl-cs.securitytoken",
                DisplayName = "HCL.CS.SECURITYTOKEN",
                Description = "Manage HCL.CS tokens",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "HclCsUser",
                ApiScopes = new List<ApiScopes>
                {
                    new()
                    {
                        Name = "hcl-cs.securitytoken.manage", DisplayName = "HCL.CS.SECURITYTOKEN.MANAGE",
                        Description = "Manage security token", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "hcl-cs.securitytoken.read", DisplayName = "HCL.CS.SECURITYTOKEN.READ",
                        Description = "Read security token", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                }
            }
        ];
    }

    public static List<IdentityResources> CreateIdentityResourceModelMaster()
    {
        return new List<IdentityResources>
        {
            new()
            {
                Name = "openid",
                DisplayName = "OPENID",
                Description = "openid",
                Enabled = true,
                Required = true,
                Emphasize = true,
                CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                Name = "profile",
                DisplayName = "PROFILE",
                Description = "profile",
                Enabled = true,
                Required = true,
                Emphasize = true,
                CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow,
                IdentityClaims = new List<IdentityClaims>
                {
                    new()
                    {
                        Type = "subject", CreatedBy = "HclCsUser", AliasType = "sub", CreatedOn = DateTime.UtcNow
                    },
                    new()
                    {
                        Type = "name", CreatedBy = "HclCsUser", AliasType = "username", CreatedOn = DateTime.UtcNow
                    },
                    new()
                    {
                        Type = "given_name", CreatedBy = "HclCsUser", AliasType = "firstname",
                        CreatedOn = DateTime.UtcNow
                    },
                    new()
                    {
                        Type = "family_name", CreatedBy = "HclCsUser", AliasType = "lastname",
                        CreatedOn = DateTime.UtcNow
                    },
                    new() { Type = "middle_name", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "nickname", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "preferred_username", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "profile", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "picture", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "website", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "Gender", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow },
                    new()
                    {
                        Type = "birthdate", CreatedBy = "HclCsUser", AliasType = "dateofbirth",
                        CreatedOn = DateTime.UtcNow
                    },
                    new() { Type = "zoneinfo", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "locale", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "updated_at", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "City", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "PinCode", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "Street", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow }
                }
            },
            new()
            {
                Name = "email",
                DisplayName = "EMAIL",
                Description = "email",
                Enabled = true,
                Required = true,
                Emphasize = true,
                CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow,
                IdentityClaims = new List<IdentityClaims>
                {
                    new() { Type = "email", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow },
                    new()
                    {
                        Type = "email_verified", AliasType = "emailconfirmed", CreatedBy = "HclCsUser",
                        CreatedOn = DateTime.UtcNow
                    }
                }
            },
            new()
            {
                Name = "phone",
                DisplayName = "PHONE",
                Description = "phone",
                Enabled = true,
                Required = true,
                Emphasize = true,
                CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow,
                IdentityClaims = new List<IdentityClaims>
                {
                    new()
                    {
                        Type = "phone_number", CreatedBy = "HclCsUser", AliasType = "phonenumber",
                        CreatedOn = DateTime.UtcNow
                    },
                    new()
                    {
                        Type = "phone_number_verified", AliasType = "phonenumberconfirmed", CreatedBy = "HclCsUser",
                        CreatedOn = DateTime.UtcNow
                    }
                }
            },
            new()
            {
                Name = "address",
                DisplayName = "ADDRESS",
                Description = "address",
                Enabled = true,
                Required = true,
                Emphasize = true,
                CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow,
                IdentityClaims = new List<IdentityClaims>
                {
                    new() { Type = "address", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow }
                }
            }
        };
    }

    public static List<Roles> CreateRolesMaster()
    {
        return new List<Roles>
        {
            new()
            {
                Description = "HclCsAdmin",
                Name = "HclCsAdmin",
                NormalizedName = "HclCsADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            },

            new()
            {
                Description = "HclCsUser",
                Name = "HclCsUser",
                NormalizedName = "HclCsUSER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            }
        };
    }

    public static List<RoleClaims> CreateRoleClaims_HclCsAdmin()
    {
        var RoleClaimsList = new List<RoleClaims>
        {
            new()
            {
                ClaimType = "permission", ClaimValue = "hcl-cs.apiResource.manage", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "hcl-cs.identityresource.manage", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "hcl-cs.client.manage", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "hcl-cs.audittrail.manage", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "hcl-cs.role.manage", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "hcl-cs.adminuser.manage", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "hcl-cs.user.write", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "hcl-cs.user.read", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "hcl-cs.securitytoken.manage", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            }
        };
        return RoleClaimsList;
    }

    public static List<RoleClaims> CreateRoleClaims_HclCsUser()
    {
        var RoleClaimsList = new List<RoleClaims>
        {
            new()
            {
                ClaimType = "permission", ClaimValue = "hcl-cs.role.read", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "hcl-cs.user.write", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "hcl-cs.user.read", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "hcl-cs.user.delete", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow
            }
        };
        return RoleClaimsList;
    }

    public static List<SecurityQuestions> CreateSecurityQuestionsModelMaster()
    {
        return new List<SecurityQuestions>
        {
            new()
            {
                Question = "What primary school did you attend?", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            },
            new()
            {
                Question = "In what town or city was your first full time job?", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow, IsDeleted = false
            },
            new()
            {
                Question = "In what town or city did you meet your spouse or partner?", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow, IsDeleted = false
            },
            new()
            {
                Question = "What are the last five digits of your driver's license number?", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow, IsDeleted = false
            },
            new()
            {
                Question = "What time of the day were you born? (hh:mm)", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow, IsDeleted = false
            },
            new()
            {
                Question = "What is the name of the place your wedding reception was held?", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow, IsDeleted = false
            },
            new()
            {
                Question = "What is the name of your favorite childhood friend?", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow, IsDeleted = false
            },
            new()
            {
                Question = "What was your childhood nickname?", CreatedBy = "HclCsUser", CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            },
            new()
            {
                Question = "What was the last name of your teacher?", CreatedBy = "HclCsUser",
                CreatedOn = DateTime.UtcNow, IsDeleted = false
            }
        };
    }

    public static UserRoles CreateUserRoleModelMaster()
    {
        return new UserRoles
        {
            ValidFrom = DateTime.UtcNow,
            ValidTo = DateTime.UtcNow.AddDays(90),
            CreatedBy = "HclCsUser",
            CreatedOn = DateTime.UtcNow
        };
    }

    public static Users CreateUserModelMaster()
    {
        return new Users
        {
            TwoFactorEnabled = false,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            RequiresDefaultPasswordChange = false,
            CreatedBy = "HclCsUser",
            CreatedOn = DateTime.UtcNow,
            TwoFactorType = TwoFactorType.None,
            SecurityStamp = "6XPYYMZYMSG73X6CMLERAU4PMQ4W4V6"
        };
    }

    public static Clients CreateClientMaster()
    {
        return new Clients
        {
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "HclCsUser",
            LogoUri = string.Empty,
            TermsOfServiceUri = string.Empty,
            PolicyUri = string.Empty,

            // Token lifetimes MUST stay within the canonical contract enforced by the backend
            // (HCL.CS.Domain.Configurations.Endpoint.TokenExpiration defaults) and the Admin UI.
            // These match HCL.CS.Domain.Models.Endpoint.ClientsModel defaults so a freshly seeded
            // Admin client can be opened and updated in the Admin UI without altering unrelated fields.
            RefreshTokenExpiration = 86400,
            AccessTokenExpiration = 900,
            IdentityTokenExpiration = 3600,
            AuthorizationCodeExpiration = 600,
            LogoutTokenExpiration = 1800,

            AccessTokenType = AccessTokenType.JWT,
            RequirePkce = true,
            IsPkceTextPlain = false,
            RequireClientSecret = true,
            IsFirstPartyApp = true,
            AllowAccessTokensViaBrowser = false,

            AllowedSigningAlgorithm = Algorithms.RsaSha256,
            AllowOfflineAccess = true,
            ApplicationType = ApplicationType.RegularWeb,

            FrontChannelLogoutSessionRequired = true,
            BackChannelLogoutSessionRequired = true
        };
    }
}
