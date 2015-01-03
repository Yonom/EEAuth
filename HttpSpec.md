# EEAuth HTTP Spec
This document describes the correct way to implement the EEAuth's login flow.   
*Note: If you are planning on using PHP, it is recommended that you use the official implementation of this specification instead of reimplementing this manually.*

## Auth request
The auth request flow will prompt a user to join a random world in EE in order to confirm their identity. A second in-game yes/no prompt prevents CSRF attacks.

### Invoking the login dialog
Your app must initiate a redirect to an endpoint which will display the login dialog:

    https://eeauth.yonom.org/login?
       client_id={api-key}
       &redirect_uri={redirect-uri}
       
This endpoint has the following required parameters:

- `client_id`. Your API key.
- `redirect_uri`. The URL that you want to redirect the person logging in back to. This URL will capture the response from the Login Dialog. 

It also has the following optional parameter:

- `state`. An arbitrary unique string created by your website to guard against [Cross-site Request Forgery](http://en.wikipedia.org/wiki/Cross-site_request_forgery).

### Handling login dialog response
At this point in the auth flow, the person will see the auth dialog and will be prompted to verify their identity.

If the person successfully finishes the verification process, their web browser returns to the `redirect_uri` provided in the request. Your website must then take the proper verification steps to make sure that the response was not edited on its way to your server. These steps are described below. An additional `state` argument is sent, if this was provided in the request.  
If the verification fails, the `redirect_uri` is invoked with the parameters `error` (for error id) and `error_description` (the error message).

## Auth response
On a successful login, the response is included inside the GET parameter `data`. A typical response looks like this:

    http://www.example.com/login.php?data=eyJleHBpcmVzIjoiMTQyMDMxMjAwOSIsInVzZXJuYW1lIjoicHJvY2Vzc29yIiwic2lnIjoiL3lWTi9BODZoUk13V3V5SW1XVExFZz09In0=

In order to parse this data, you must first base64 decode it. The result is a JSON object which looks like this:
     
    {
        "expires":"1420312000",
        "username":"processor",
        "sig":"/yVN/A86hRMwWuyImWTLEg=="
    }

This object contains the following (required) fields:

- **username:** the username of the verified user
- **expires:** unix time in UTC, when this login token expires
- **sig:** the signature to verify the integrity of this response


### Confirming identity
Because this redirect flow involves browsers being redirected to URLs in your app from the Login dialog, traffic could directly access this URL with made-up fragments or parameters. If your website assumed these were valid parameters, the made-up data would be used by your website for potentially malicious purposes. As a result, your website should confirm that the person using the website is the same person that you have response data for before accepting the provided username.

#### Verification

The following steps must be taken in order to verify the request:

- Calculate a valid sig for the received response
- Make sure the received `sig` parameter matches the calculated one
- Check the server time and make sure the request has not yet expired

#### Calculating the signature

Calculate the signature of the request using your api secret. Calculating the signature is done in parts: sort the parameters received plus your redirect\_uri alphabetically, join into a string resulting in key=valuekey2=value2, calculate the hmac\_md5 of the string and your private key.

This HMAC is used for the sig parameter in the request therefore it should not be be calculated with sig as a parameter.

Pseudo code:

	args = all query parameters received and the redirect_uri 
	       (e.g. username, expires, redirect_uri) excluding sig.
	
	args_sorted = sort_args_alphabetically_by_key(args)
	
	args_concat = join(args_sorted) 
	
	# Output: expires=14203120008redirect_uri=http://www.example.com/login.php
	#         username=processor
	
	sig = hmacmd5(api_secret, args_concat)


