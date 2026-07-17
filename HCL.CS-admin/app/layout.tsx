import type { Metadata } from "next";
import { DM_Sans, JetBrains_Mono } from "next/font/google";

import { Providers } from "@/components/providers";
import { auth } from "@/lib/auth";

import "./globals.css";

export const metadata: Metadata = {
  title: "HCL.CS Admin",
  description: "Administrative console for HCL.CS identity platform"
};

const fontDisplay = DM_Sans({
  subsets: ["latin"],
  display: "swap",
  weight: ["500", "600", "700"],
  variable: "--font-display"
});

const fontBody = DM_Sans({
  subsets: ["latin"],
  display: "swap",
  weight: ["400", "500", "600", "700"],
  variable: "--font-body"
});

const fontMono = JetBrains_Mono({
  subsets: ["latin"],
  display: "swap",
  variable: "--font-mono"
});

export default async function RootLayout({ children }: { children: React.ReactNode }) {
  const session = await auth();

  return (
    <html
      lang="en"
      data-theme="light"
      suppressHydrationWarning
      className={`${fontDisplay.variable} ${fontBody.variable} ${fontMono.variable}`}
    >
      <head>
        <script
          dangerouslySetInnerHTML={{
            __html:
              "try{document.documentElement.dataset.theme=localStorage.getItem('hcl-cs-theme')||'light'}catch(e){}"
          }}
        />
      </head>
      <body>
        <Providers session={session}>{children}</Providers>
      </body>
    </html>
  );
}
