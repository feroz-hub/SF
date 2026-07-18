/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use client";

import { motion } from "framer-motion";

export function AmbientBackground() {
  return (
    <>
      <div className="future-ambient future-ambient-base" />
      <div className="future-ambient future-ambient-color" />
      <motion.div
        aria-hidden="true"
        className="future-ambient future-ambient-motion"
        animate={{ opacity: [0.34, 0.62, 0.34] }}
        transition={{ duration: 22, repeat: Infinity, repeatType: "mirror" }}
      />
      <div className="future-ambient future-ambient-grid" />
    </>
  );
}
