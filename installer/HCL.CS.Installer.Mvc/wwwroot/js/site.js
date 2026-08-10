/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

document.addEventListener("DOMContentLoaded", () => {
    document.body.classList.add("is-ready");

    document.querySelectorAll(".js-loading-form").forEach((form) => {
        form.addEventListener("submit", () => {
            form.querySelectorAll("button[type='submit']").forEach((button) => {
                button.setAttribute("disabled", "disabled");
            });

            form.querySelectorAll(".js-loading-indicator").forEach((indicator) => {
                indicator.classList.remove("d-none");
            });
        });
    });

    const defaultScopesCheckbox = document.getElementById("UseDefaultScopes");
    const allowedScopesInput = document.getElementById("AllowedScopes");

    const syncAllowedScopesState = () => {
        if (!defaultScopesCheckbox || !allowedScopesInput) {
            return;
        }

        const disabled = defaultScopesCheckbox.checked;
        allowedScopesInput.disabled = disabled;
        allowedScopesInput.closest(".field-block")?.classList.toggle("is-disabled", disabled);
    };

    if (defaultScopesCheckbox && allowedScopesInput) {
        syncAllowedScopesState();
        defaultScopesCheckbox.addEventListener("change", syncAllowedScopesState);
    }

    const syncChoiceCards = () => {
        document.querySelectorAll(".choice-check").forEach((card) => {
            const input = card.querySelector(".form-check-input[type='checkbox']");
            if (!input) {
                return;
            }

            card.classList.toggle("is-selected", input.checked);
        });
    };

    document.querySelectorAll(".choice-check .form-check-input[type='checkbox']").forEach((input) => {
        input.addEventListener("change", syncChoiceCards);
    });
    syncChoiceCards();

    const fallbackCopy = (value) => {
        const textarea = document.createElement("textarea");
        textarea.value = value;
        textarea.style.position = "fixed";
        textarea.style.opacity = "0";
        document.body.appendChild(textarea);
        textarea.focus();
        textarea.select();

        let copied = false;
        try {
            copied = document.execCommand("copy");
        } catch {
            copied = false;
        }

        textarea.remove();
        return copied;
    };

    const flashCopyState = (button, label) => {
        const original = button.dataset.originalLabel || button.textContent;
        button.dataset.originalLabel = original;
        button.textContent = label;

        window.setTimeout(() => {
            button.textContent = original;
        }, 1400);
    };

    document.querySelectorAll("[data-copy-target], [data-copy-value]").forEach((button) => {
        button.addEventListener("click", async () => {
            const explicitValue = button.getAttribute("data-copy-value");
            const targetId = button.getAttribute("data-copy-target");
            const target = targetId ? document.getElementById(targetId) : null;
            const targetValue = target ? target.textContent?.trim() : "";
            const value = (explicitValue ?? targetValue ?? "").trim();

            if (!value) {
                flashCopyState(button, "No value");
                return;
            }

            try {
                if (navigator.clipboard && window.isSecureContext) {
                    await navigator.clipboard.writeText(value);
                    flashCopyState(button, "Copied");
                    return;
                }

                const copied = fallbackCopy(value);
                flashCopyState(button, copied ? "Copied" : "Copy failed");
            } catch {
                const copied = fallbackCopy(value);
                flashCopyState(button, copied ? "Copied" : "Copy failed");
            }
        });
    });
});
