<?php

class EEAuth
{
    private $api_url = 'http://eeauth.yonom.org/login';
    private $api_key;
    private $api_secret;
    private $redirect_uri;

    public function __construct($api_key, $api_secret, $redirect_uri) {
        $this->api_key = $api_key;
        $this->api_secret = $api_secret;
        $this->redirect_uri = $redirect_uri;
    }

    public function getLoginUrl($state = false) {
        return $this->api_url .
            '?client_id=' . $this->api_key .
            '&redirect_uri=' . $this->redirect_uri .
            ($state ? ('&state=' . $state) : '');
    }

    public function verifyLogin($data) {
        $jsonStr = base64_decode($data);
        $json = json_decode($jsonStr, true);
        $json['redirect_uri'] = $this->redirect_uri;
        $sig = $json['sig'];
        unset($json['sig']);

        if ($this->calcSig($json) !== $sig)
            throw new InvalidArgumentException("The given data is invalid.");

        if ($json['expires'] - time() < 0)
            throw new InvalidArgumentException("Login has expired.");

        $response = array(
            'name' => $json['name']
        );
        if (isset($json['state'])){
            $response['state'] = $json['state'];
        }

        return $response;
    }

    private function calcSig($params) {
        ksort($params);
        $param_string = '';
        foreach ($params as $param => $value) {
            $param_string .= $param . '=' . $value;
        }
        return base64_encode(hash_hmac('md5', $param_string, $this->api_secret, true));
    }
}