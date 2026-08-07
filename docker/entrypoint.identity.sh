#!/usr/bin/env bash
set -euo pipefail

APP_DIR="/app"
HTTPS_DIR="${HCL_CS_HTTPS_DIR:-${APP_DIR}/https}"
HTTPS_CERT_PATH="${HCL_CS_HTTPS_CERT_PATH:-${HTTPS_DIR}/hcl-cs-devcert.pfx}"
HTTPS_CERT_PASSWORD="${HCL_CS_HTTPS_CERT_PASSWORD:-hcl-cs-dev-cert}"
HTTPS_CERT_PEM="${HTTPS_DIR}/hcl-cs-devcert.crt"
HTTPS_KEY_PEM="${HTTPS_DIR}/hcl-cs-devcert.key"
ASPNETCORE_URLS_VALUE="${ASPNETCORE_URLS:-}"

log() {
  printf '[hcl-cs-entrypoint] %s\n' "$*"
}

fail() {
  printf '[hcl-cs-entrypoint] ERROR: %s\n' "$*" >&2
  exit 1
}

require_environment_value() {
  local variable_name="$1"
  [[ -n "${!variable_name:-}" ]] ||
    fail "Required environment variable '${variable_name}' is not configured."
}

validate_signing_certificate() {
  local algorithm="$1"
  local path_variable="$2"
  local base64_variable="$3"
  local certificate_path="${!path_variable:-}"
  local certificate_base64="${!base64_variable:-}"

  if [[ -z "${certificate_path}" && -z "${certificate_base64}" ]]; then
    fail "A persistent ${algorithm} signing certificate is required. Configure '${path_variable}' or '${base64_variable}'."
  fi

  if [[ -n "${certificate_path}" && ! -r "${certificate_path}" ]]; then
    fail "The ${algorithm} signing certificate configured by '${path_variable}' is not readable."
  fi
}

validate_runtime_configuration() {
  require_environment_value "HCL_CS_DB_CONNECTION_STRING"
  require_environment_value "HCL_CS_SIGNING_CERT_PASSWORD"
  validate_signing_certificate \
    "RS256" \
    "HCL_CS_RSA_SIGNING_CERT_PATH" \
    "HCL_CS_RSA_SIGNING_CERT_BASE64"
  validate_signing_certificate \
    "ES256" \
    "HCL_CS_ECDSA_SIGNING_CERT_PATH" \
    "HCL_CS_ECDSA_SIGNING_CERT_BASE64"

  log "Required runtime and persistent signing-certificate configuration is present."
}

configure_binding_url() {
  if [[ -n "${PORT:-}" ]] && [[ -z "${ASPNETCORE_URLS_VALUE}" || "${ASPNETCORE_URLS_VALUE}" == "https://+:8443" ]]; then
    ASPNETCORE_URLS_VALUE="http://+:${PORT}"
    export ASPNETCORE_URLS="${ASPNETCORE_URLS_VALUE}"
    log "Detected Railway-style PORT=${PORT}; binding HCL.CS over internal HTTP."
  fi
}

ensure_https_certificate() {
  if [[ "${ASPNETCORE_URLS_VALUE}" != *"https://"* ]]; then
    log "ASPNETCORE_URLS does not require HTTPS. Skipping local Kestrel certificate setup."
    return 0
  fi

  local kestrel_cert_path="${ASPNETCORE_Kestrel__Certificates__Default__Path:-${HTTPS_CERT_PATH}}"
  local kestrel_cert_password="${ASPNETCORE_Kestrel__Certificates__Default__Password:-${HTTPS_CERT_PASSWORD}}"

  export ASPNETCORE_Kestrel__Certificates__Default__Path="${kestrel_cert_path}"
  export ASPNETCORE_Kestrel__Certificates__Default__Password="${kestrel_cert_password}"

  mkdir -p "$(dirname "${kestrel_cert_path}")"

  if [[ -f "${kestrel_cert_path}" ]]; then
    log "Using existing HTTPS certificate: ${kestrel_cert_path}"
    return 0
  fi

  log "Generating self-signed HTTPS certificate for HCL.CS."
  openssl req \
    -x509 \
    -nodes \
    -newkey rsa:2048 \
    -keyout "${HTTPS_KEY_PEM}" \
    -out "${HTTPS_CERT_PEM}" \
    -days 365 \
    -subj "/CN=localhost" \
    -addext "subjectAltName=DNS:localhost,IP:127.0.0.1"

  openssl pkcs12 \
    -export \
    -out "${kestrel_cert_path}" \
    -inkey "${HTTPS_KEY_PEM}" \
    -in "${HTTPS_CERT_PEM}" \
    -password "pass:${kestrel_cert_password}"

  log "HTTPS certificate generated at ${kestrel_cert_path}."
}

main() {
  configure_binding_url
  validate_runtime_configuration
  ensure_https_certificate
  exec dotnet HCL.CS.DemoServerApp.dll
}

main "$@"
