/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import { signIn, signOut, useSession } from "next-auth/react";
import { useRouter } from "next/navigation";
import { useSearchParams } from "next/navigation";

import { Button } from "@/components/ui/button";
import { AuthShell } from "@/components/layout/AuthShell";

const defaultCallbackPath = "/admin/clients";

function mapLoginError(rawError: string | null): string | null {
  if (!rawError) {
    return null;
  }

  switch (rawError) {
    case "Configuration":
      return "Authentication configuration error.";
    case "AccessDenied":
      return "Access denied.";
    case "OAuthSignin":
    case "OAuthCallback":
    case "OAuthCreateAccount":
      return "HCL.CS sign-in could not be completed. Please try again.";
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
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [startingLogin, setStartingLogin] = useState(false);

  useEffect(() => {
    if (status === "authenticated" && reasonAdminRequired) {
      void signOut({ redirect: false });
      return;
    }
    if (status === "authenticated") {
      router.replace(callbackUrl);
    }
  }, [callbackUrl, router, status, reasonAdminRequired]);

  const startLogin = async () => {
    if (startingLogin || status === "loading") {
      return;
    }

    setStartingLogin(true);
    setSubmitError(null);

    try {
      await signIn("hcl-cs", { callbackUrl });
    } catch {
      setSubmitError("Unable to start HCL.CS sign-in. Please try again.");
      setStartingLogin(false);
    }
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

      <Button type="button" onClick={startLogin} disabled={status === "loading" || startingLogin}>
        {status === "loading"
          ? "Checking session..."
          : startingLogin
            ? "Redirecting to HCL.CS..."
            : "Sign in with HCL.CS"}
      </Button>

      <p className="login-help-link">
        <Link href="/login/forgot-password" className="link">
          Forgot password?
        </Link>
      </p>
    </AuthShell>
  );
}
