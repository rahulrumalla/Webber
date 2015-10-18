# Webber
========
<a href="https://www.nuget.org/packages/Webber"><img src="https://img.shields.io/nuget/v/Webber.svg" alt="NuGet Version" /></a> 
<a href="https://www.nuget.org/packages/Webber"><img src="https://img.shields.io/nuget/dt/Webber.svg" alt="NuGet Download Count" /></a>

### Overview
Webber is a lightweight, HTTP Web Request Helper / utility written in C#. 
The idea was to have a very simple and flexible abstraction of doing HTTP requests with minimal lines of code to perform the request. 

### Usage
####GET
Performing a GET request is as simple as 
```csharp
var response = Webber.Get<SamplePost>("http://jsonplaceholder.typicode.com/posts/1");
```

####POST
Shoot a POST request with a wide-range of flexibility
```csharp
//Simple POST
var webberResponse = Webber.Post<SamplePost>(url, data);

//Get flexible control on overriding the default parameters
var webberResponse = Webber.Post<Tests.SamplePost>(
                url,
                data,
                ContentType.Json,
                EncodingType.Utf8,
                new NetworkCredential("myUserName", "P@ssW0rd"),
                myCustomerHeaders);
```

####Any HTTP Verb
```csharp
var webberResponse = Webber.Invoke(
                url,
                data,
                ContentType.Json, // Specify the request's content-type
                MethodType.Put, // Specify the http verb here
                EncodingType.Utf8, // Specify the request's encoding type here
                new NetworkCredential("myUserName", "P@ssW0rd"), // Add your ICrendential when necessary
                myCustomerHeaders); // Add additional headers the reqest
```

###Response Deserialization
These web requests' responses can be auto-deserialized from a Json. The library only has 1 dependency, that is Json.Net. Here's a good article on how Json.net is better than the built in .net serializers http://www.newtonsoft.com/json/help/html/jsonnetvsdotnetserializers.htm
