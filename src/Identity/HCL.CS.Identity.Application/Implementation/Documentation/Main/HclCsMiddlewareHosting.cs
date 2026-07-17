/**
* @page HclCsMiddlewareHosting HCL.CS Middleware Hosting
* <p>Middleware in ASP.NET Core controls how the application responds to HTTP requests. It can also control, authenticate and authorize a user to perform specific actions.</p>
* <ul>
* <li>Middleware are software components that are assembled into an application pipeline to handle requests and responses.</li>
* <li>Can perform work before and after the next component in the pipeline.</li>
* <li>Request delegates are used to build the request pipeline. The request delegates handle each HTTP request.</li>
* <li>Request delegates are configured using&nbsp; Run,&nbsp; Map , and&nbsp; Use extension methods.</li>
* <li>Each middleware component in the request pipeline is responsible for invoking the next component in the pipeline or short-circuiting the pipeline.</li>
* <li>When a middleware short-circuits, it's called a&nbsp;terminal middleware&nbsp;because it prevents further middleware from processing the request.</li>
* </ul>
* <p>HCL.CS has middleware extension which is created using <strong>IApplicationBuilder</strong>, it can be consumed in the middleware hosting. Once HCL.CS middleware extension component integrated into target application, and then it is capable to handle OAuth/API request and responses via HTTPContext.</p>
*/



