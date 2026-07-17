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
