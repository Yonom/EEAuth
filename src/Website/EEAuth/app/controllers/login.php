<?php

$data['title'] = "Login";
$data['page'] = "api";

$data['name'] = isset($_GET['app_name'])
    ? $_GET['app_name']
    : parse($_GET['redirect_uri']);

function parse($url){
    if(strpos($url,"://")===false && substr($url,0,1)!="/") $url = "http://".$url;
    $info = parse_url($url);
    return $info['host'];
}