<?php include ("EEAuth.php");

$public_key = <INSERT_PUBLIC_KEY_HERE>;
$private_key = <INSERT_PRIVATE_KEY_HERE>;
$redirect_uri = <INSERT_REDIRECT_URI_HERE>; // URI of this page
$eeAuth = new EEAuth($public_key, $private_key, $redirect_uri);

if (isset($_GET['error'])) { // If there is an error
    echo 'Error: ' . $_GET['error_description'];
    die();
} elseif (!isset($_GET['data'])) { // If we are visiting for the first time
    header("Location: " . $eeAuth->getLoginUrl());
    die();
}

try {
    $loginData = $eeAuth->verifyLogin($_GET['data']); // This throws an exception if the login is invalid
    $username = $loginData['name'];
    $connectuserid = $loginData['connectuserid'];

    // TODO Set the session tokens here
} catch (Exception $ex) {
    echo 'Error: ' . $ex->getMessage();
}
