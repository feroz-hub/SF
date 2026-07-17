/**
* @page SoftwareSystemArchitecture Software System Architecture
* <p>The software architecture definition serves as the basis for the detailed design and implementation of all software components. This document provides a comprehensive architectural overview of the Zentra and is intended to capture and convey key architectural decisions.</p>
* <p>&nbsp;Below diagram represents the overall Zentra architecture and its components&rsquo; interaction with each other.</p>
* \image html SoftwareArchitecture.jpg
* <p><center><strong> Zentra &ndash; High Level Architecture </strong></center></p>
* <p>This software architecture document has several purposes. It helps&nbsp; development, quality and project teams&nbsp; understand the principles of architecture. It also helps the document reader&nbsp; understand the constraints placed on the architecture and how the architecture has been conceptualized.</p>
* <p>The Zentra uses Microsoft ASP.NET Identity as the foundation to manage user authentication, roles, and user profiles. ASP.NET Identity Framework feature is customized as per industry standards to meet security objectives.</p>
* <p>Below are some of the customizations done in ASP.NET identity</p>
* <ul>
* <li>Replaced existing PBDKF2 hash with Argon2</li>
* <li>Soft deletion of records instead of hard delete</li>
* <li>Password reuse restrictions have been introduced.</li>
* <li>Security questionnaire is introduced as additional factor for managing user accounts</li>
* <li>User Authentication check for API calls</li>
* </ul>
* <p>&nbsp;In addition to identity management, Zentra exposes OAuth2.0 and OpenID Connect compliant endpoints to handle user authentication and token management.</p>

*/


