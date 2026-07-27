# LDAP authentication configuration

LDAP authentication is disabled by default. HCL.CS enables local authentication
only while LDAP is disabled or lacks mandatory configuration. When LDAP is
enabled and fully configured, every password login uses the directory and local
passwords are unavailable.

Configure production values through the deployment secret provider or environment. Never commit the service-account
password. The demo server supports these environment variables:

```text
HCL_CS_LDAP__ENABLED
HCL_CS_LDAP__HOST
HCL_CS_LDAP__PORT
HCL_CS_LDAP__USESSL
HCL_CS_LDAP__USESTARTTLS
HCL_CS_LDAP__BASEDN
HCL_CS_LDAP__USERSEARCHBASE
HCL_CS_LDAP__USERSEARCHFILTER
HCL_CS_LDAP__BINDDN
HCL_CS_LDAP__BINDPASSWORD
HCL_CS_LDAP__CONNECTTIMEOUTSECONDS
HCL_CS_LDAP__SEARCHTIMEOUTSECONDS
HCL_CS_LDAP__REQUIREEMPLOYEEID
HCL_CS_LDAP__REQUIREDEPARTMENT
HCL_CS_LDAP__REQUIREUSERPRINCIPALNAME
HCL_CS_LDAP__ATTRIBUTES__IMMUTABLEID
HCL_CS_LDAP__ATTRIBUTES__EMPLOYEEID
HCL_CS_LDAP__ATTRIBUTES__USERPRINCIPALNAME
HCL_CS_LDAP__ATTRIBUTES__EMAIL
HCL_CS_LDAP__ATTRIBUTES__DISPLAYNAME
HCL_CS_LDAP__ATTRIBUTES__DEPARTMENT
HCL_CS_LDAP__ATTRIBUTES__ACCOUNTSTATUS
```

Example placeholders:

```text
HCL_CS_LDAP__ENABLED=true
HCL_CS_LDAP__HOST=ldap.example.internal
HCL_CS_LDAP__PORT=636
HCL_CS_LDAP__USESSL=true
HCL_CS_LDAP__USESTARTTLS=false
HCL_CS_LDAP__BASEDN=DC=example,DC=internal
HCL_CS_LDAP__USERSEARCHBASE=OU=Users,DC=example,DC=internal
HCL_CS_LDAP__USERSEARCHFILTER=(userPrincipalName={0})
HCL_CS_LDAP__BINDDN=CN=ldap-reader,OU=Service Accounts,DC=example,DC=internal
HCL_CS_LDAP__BINDPASSWORD=<injected-secret>
```

Use either LDAPS or StartTLS, never both. Server certificates use the operating system or container trust store and
must pass chain and hostname validation. Import a development or enterprise CA into that trust store; application code
does not provide a certificate-validation bypass.

Unencrypted LDAP is rejected. The only exception is an explicitly configured loopback test server using
`AllowUnencryptedForDevelopment`; this must not be enabled in a shared or production environment.

The default immutable identifier is Active Directory `objectGUID`. Its 16 binary bytes are converted with
`new Guid(bytes).ToString("D")`, producing the same canonical identifier on every login. Attribute names are
centralized under `SystemSettings:LdapConfig:Attributes`. Email, display name, immutable ID, and an active account
status are mandatory. Employee ID, department, and user principal name can be made mandatory with their respective
flags.

Unit tests use a protocol fake and do not contact LDAP. Validate LDAPS/StartTLS, service bind, account status, HCL
attribute names, concurrent binds, and connection cleanup against an approved non-production HCL directory before
enabling the feature.

## Local authentication mode

HCL.CS resolves authentication mode on the server. LDAP is used only when
`HCL_CS_LDAP__ENABLED=true` and all mandatory LDAP settings are present. When
LDAP is explicitly disabled or mandatory settings are absent, local registration
is available only for the exact `hcltech.com` email domain. An LDAP timeout, TLS
failure, bind failure, or directory outage never changes the resolved mode and
never falls back to a local password.

Optional environment overrides:

```text
HCL_CS_LOCALAUTHENTICATION__ENABLEDWHENLDAPDISABLEDORUNCONFIGURED=true
HCL_CS_LOCALAUTHENTICATION__ALLOWEDEMAILDOMAINS=hcltech.com
HCL_CS_LOCALAUTHENTICATION__REQUIREEMAILCONFIRMATION=true
HCL_CS_LOCALAUTHENTICATION__ALLOWSELFREGISTRATION=true
HCL_CS_LOCALAUTHENTICATION__ALLOWADMINISTRATORCREATION=true
```

Local passwords are hashed by ASP.NET Core Identity. Email-confirmation and
lockout lifetimes use `UserConfig.EmailTokenExpiry`,
`UserConfig.DefaultLockoutTimeSpanMin`, and
`UserConfig.MaxFailedAccessAttempts`. SMTP and LDAP secrets must be injected
through the existing environment or secret placeholder mechanism.
