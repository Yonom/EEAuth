<?php

$data['title'] = "Generate API Key";
$data['page'] = "api";

set_error_handler(function($errno, $errstr, $errfile, $errline, array $errcontext) {
    if (0 === error_reporting()) {
        return false;
    }
    throw new ErrorException($errstr, 0, $errno, $errfile, $errline);
});

try {
    $jsonPath = 'http://localhost:5011/getkey';
    $jsonStr = file_get_contents($jsonPath);
    $json = json_decode($jsonStr, true);
    $data['publicKey'] = $json['PublicKey'];
    $data['privateKey'] = $json['PrivateKey'];
} catch (Exception $ex) {
    $path[0] = 'error';
    $data['errorDescription'] = "Unable to communicate with worker process.";
}
restore_error_handler();