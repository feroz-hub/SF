/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use client";

import Link from "next/link";
import { useEffect, useMemo, useRef, useState } from "react";
import { signIn, signOut, useSession } from "next-auth/react";
import { useRouter } from "next/navigation";
import { useSearchParams } from "next/navigation";
import { Eye, EyeOff } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { AuthShell } from "@/components/layout/AuthShell";
import { env } from "@/lib/env";

const defaultCallbackPath = "/admin/clients";

function mapLoginError(rawError: string | null): string | null {
  if (!rawError) {
    return null;
  }

  switch (rawError) {
    case "CredentialsSignin":
      return "Invalid username or password.";
    case "Configuration":
      return "Authentication configuration error.";
    case "AccessDenied":
      return "Access denied.";
    default:
      return rawError;
  }
}

function sanitizeCallbackUrl(rawCallbackUrl: string | null): string {
  if (!rawCallbackUrl) {
    return defaultCallbackPath;
  }

  if (rawCallbackUrl.startsWith("/") && !rawCallbackUrl.startsWith("//")) {
    return rawCallbackUrl;
  }

  if (typeof window === "undefined") {
    return defaultCallbackPath;
  }

  try {
    const parsed = new URL(rawCallbackUrl);
    if (parsed.origin === window.location.origin) {
      return `${parsed.pathname}${parsed.search}${parsed.hash}`;
    }
  } catch {
    return defaultCallbackPath;
  }

  return defaultCallbackPath;
}

export default function LoginPage() {
  const { status } = useSession();
  const router = useRouter();
  const searchParams = useSearchParams();
  const callbackUrl = useMemo(
    () => sanitizeCallbackUrl(searchParams.get("callbackUrl")),
    [searchParams]
  );
  const urlError = useMemo(() => mapLoginError(searchParams.get("error")), [searchParams]);
  const reasonAdminRequired = useMemo(
    () => searchParams.get("reason") === "admin_required",
    [searchParams]
  );
  const userCode = useMemo(() => searchParams.get("UserCode") ?? "", [searchParams]);
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [startingLogin, setStartingLogin] = useState(false);
  const userCodeAttempted = useRef(false);

  useEffect(() => {
    if (status === "authenticated" && reasonAdminRequired) {
      void signOut({ redirect: false });
      return;
    }
    if (status === "authenticated") {
      router.replace(callbackUrl);
    }
  }, [callbackUrl, router, status, reasonAdminRequired]);

  useEffect(() => {
    if (status !== "unauthenticated" || !userCode || userCodeAttempted.current || startingLogin) {
      return;
    }
    userCodeAttempted.current = true;
    setSubmitError(null);
    signIn("credentials", {
      code: userCode,
      callbackUrl,
      redirect: false
    }).then((result) => {
      if (result?.ok) {
        router.replace(callbackUrl);
      } else {
        setSubmitError(
          result?.error === "CredentialsSignin"
            ? "Sign-in link expired or already used. Please try again."
            : (result?.error ?? "Sign-in failed.")
        );
        userCodeAttempted.current = false;
      }
    });
  }, [userCode, status, callbackUrl, router, startingLogin]);

  const startLogin = async () => {
    if (startingLogin || status === "loading") {
      return;
    }

    if (!userName.trim() || !password) {
      setSubmitError("Username and password are required.");
      return;
    }

    setStartingLogin(true);
    setSubmitError(null);

    const result = await signIn("credentials", {
      username: userName.trim(),
      password,
      callbackUrl,
      redirect: false
    });

    if (result?.ok) {
      router.replace(callbackUrl);
      return;
    }

    const message = mapLoginError(result?.error ?? null) ?? "Login failed.";
    setSubmitError(message);
    setStartingLogin(false);
  };

  return (
    <AuthShell>
      <div className="login-card-heading">
        <p className="kicker">HCL.CS Administration</p>
        <h1>Welcome back</h1>
        <p>Sign in to continue to the HCL.CS Admin Console.</p>
      </div>

      {reasonAdminRequired ? (
        <p className="inline-message" style={{ marginBottom: "0.5rem" }}>
          Your session doesn&apos;t have administrator access. Please sign in with an account that
          has the admin role.
        </p>
      ) : null}
      {urlError ? <p className="inline-error">Login error: {urlError}</p> : null}
      {submitError ? <p className="inline-error">{submitError}</p> : null}
      {status === "authenticated" ? (
        <p className="inline-success">Session active. Redirecting...</p>
      ) : null}

      <div className="form-grid">
        <div className="form-row">
          <label htmlFor="username">Username</label>
          <Input
            id="username"
            autoComplete="username"
            value={userName}
            onChange={(event) => setUserName(event.target.value)}
            disabled={status === "loading" || startingLogin}
          />
        </div>

        <div className="form-row">
          <label htmlFor="password">Password</label>
          <div className="password-field">
            <input
              id="password"
              className="input password-field-input"
              type={showPassword ? "text" : "password"}
              autoComplete="current-password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              onKeyDown={(event) => {
                if (event.key === "Enter") {
                  void startLogin();
                }
              }}
              disabled={status === "loading" || startingLogin}
            />
            <button
              type="button"
              className="password-field-toggle"
              onClick={() => setShowPassword((current) => !current)}
              aria-label={showPassword ? "Hide password" : "Show password"}
              aria-controls="password"
              aria-pressed={showPassword}
              disabled={status === "loading" || startingLogin}
            >
              {showPassword ? (
                <EyeOff size={18} strokeWidth={1.8} />
              ) : (
                <Eye size={18} strokeWidth={1.8} />
              )}
            </button>
          </div>
        </div>
      </div>

      <Button type="button" onClick={startLogin} disabled={status === "loading" || startingLogin}>
        {status === "loading" ? "Checking session..." : startingLogin ? "Signing in..." : "Sign in"}
      </Button>

      <div className="login-alternate">
        <div className="login-divider">
          <span>or continue with</span>
        </div>
        <p className="inline-message">
          Google sign-in requires the HCL.CS Demo Server to be enabled.
        </p>
        <Button
          type="button"
          variant="secondary"
          style={{ width: "100%" }}
          disabled={status === "loading" || startingLogin}
          onClick={() => {
            const base = env.demoServerBaseUrl.replace(/\/+$/, "");
            const returnUrl = encodeURIComponent(
              (typeof window !== "undefined" ? window.location.origin : "") + "/login"
            );
            window.location.href = `${base}/auth/external/google/start?returnUrl=${returnUrl}`;
          }}
        >
          Sign in with Google
        </Button>
      </div>

      <p className="login-help-link">
        <Link href="/login/forgot-password" className="link">
          Forgot password?
        </Link>
      </p>
    </AuthShell>
  );
}
