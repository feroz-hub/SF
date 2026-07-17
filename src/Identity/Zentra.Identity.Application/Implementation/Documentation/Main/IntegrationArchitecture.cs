/**
* @page IntegrationArchitecture System Integration Architecture
* <p>The System Integration Architecture depicted below provides details on how a target server application can integrate to the various components of the Zentra.</p>
* <p>The Zentra exposes the features in the form of reusable libraries for applications that interface with the target servers. For user authentication and token management, the target application must consume OAuth/OpenID Connect compliant endpoints.&nbsp; Zentra also exposes APIs, that are mainly used for administrative and user registration purpose, it can be consumed via Restful services. API has predefined scopes; it must be assigned to user via roles to prevent unauthorized access.&nbsp;Framework throws an error if the user does not have claims or scope for the requested API.</p>
* <p>The Zentra requires specific tables in the target application's data store to maintain framework-related data including user, claims, roles, and API resources etc.&nbsp; When no tables are present in the data store of the target application, these tables and schema with master data will be generated on the first use utilizing the "code first" approach. Otherwise, pre-generated SQL scripts can be used to create these tables and schemas with master data as part of the deployment of the target application.</p>
* \image html IntegrationArchitecture.jpg
* <p><center><strong> Zentra &ndash; Integration Architecture </strong></center></p>
* <p>Above diagram represent how the Zentra will be integrated into target application. The Zentra acting as backend service can focus on implementing OAuth 2.0 and OpenID Connect&nbsp; without caring about other components such as identity management, user authentication, login session management and API management. Zentra and its related APIs can be accessed from target application via middleware and API wrapper(s), here middleware acts as bridge between the client application and the Zentra to manage user access using JWT tokens.</p>
* <p>ASP.NET Core introduced a new concept called <strong> Middleware </strong> ; it is nothing but a component (class) which is executed on every request in ASP.NET Core application. Middleware can be built-in as part of the .NET Core framework, added via NuGet packages, or can be custom middleware. These middleware components are configured as part of the application startup class in the configure method. Configure methods set up a request processing pipeline for an ASP.NET Core application.</p>
* <p>Zentra has DI extension support, using this Zentra can be integrated into Middleware with necessary configurations. Configuration parameters are mandatory to execute the Zentra as expected, configuration parameters includes token settings, user account default parameters, certificates for token singing, email/SMS settings, database configuration etc. If the mandatory configuration parameters are not provided, Zentra initialization throws an error(s) and aborts.</p>
* <p>Zentra is also able to interface with following external components based on initial configuration parameters,</p>
* <ol>
* <li>SMTP Server &ndash; for sending email notifications for account and system state changes</li>
* <li>LDAP &ndash; for user authentication from organization domain</li>
* <li>Twilio &ndash; for sending OTP for user activation and account management such as forget password</li>
* <li>Authenticator App &ndash; Support Microsoft and Google Authenticator app to generate time bound multifactor authentication token (MFA) to authenticate user.</li>
* </ol>
*/


