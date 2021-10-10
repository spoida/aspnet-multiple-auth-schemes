Demonstration of using multiple authentication schemes to authenticate and authorize different controllers.

There are two authentication schemes available. To authenticate requests, supply an HTTP Authorization header with one of the following values:

1. Authorization: `AuthSchemeOne 123`
2. Authorization: `AuthSchemeTwo secure`

### Scenario

The goals of this example is to:

1. Automatically authorize all requests by default, except those attributed with `AllowAnonymous`:
   1. The `AnonController` allows all requests regardless of authentication status.
   2. The `FallbackController` doesn't explicitly specify any authorization, however a `FallbackPolicy` is in place, which means any requests missing `[Authorize]/[AllowAnonymous]` will use the fallback policy.
2. Restrict access to a controller to only allow requests that have authenticated using `AuthSchemeOne`:
   1. The `AuthSchemeController` only allows authenticated requests using `AuthSchemeOne` so even requests that authenticate successfully via AuthSchemeTwo will be rejected.
3. Handle the case when an explicit empty `[Authorize]` attribute is applied to a controller:
   1. The `DefaultController` specifies an empty `[Authorize]` attribute, however no `DefaultPolicy` has been setup. A default authentication scheme has been setup. In this case, the FallbackPolicy is invoked so requests that are authenticated and satisfy the fallback policy will succeed.

An important aspect of this solution is _not_ setting a default authorization policy. When a default authorization policy is specified, it interferes with any authorization attributes that are restricted to schemes other than the default.

You can see this in action if you uncomment the following line in Startup.cs
`options.DefaultPolicy = fallbackPolicy;`. Then authenticate using `AuthSchemeTwo` and try requesting the `AuthSchemeController`. You will see that the request will succeed even though the controller says to _only_ authorize AuthSchemeOne. This happens because the controller effectively has _two_ `[Authorize]` attributes applied; the explicit one set i.e. `[Authorize(AuthenticationSchemes = AuthSchemeOneHandler.AuthSchemeOne)]`, and an empty `[Authorize]` attribute "added" by the default authorization policy. This is a subtle, unexpected behaviour that results from mixing default and fallback schemes and policies.