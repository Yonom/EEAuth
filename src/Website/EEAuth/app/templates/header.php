<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <title><?= $data['title'] ?></title><div class="background-image"></div>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="/css/main.css">
</head>
<body>
<div class="site-wrapper">
<div class="site-wrapper-inner">
<div class="cover-container">
<div class="masthead clearfix">
    <div class="inner">
        <h3 class="masthead-brand"><a href="/" style="text-decoration: none">EEAuth</a></h3>
        <nav>
            <ul class="nav masthead-nav">
                <li <? if ($data['page'] == 'home') echo 'class="active"'; ?>><a href="/">Home</a></li>
                <li <? if ($data['page'] == 'api') echo 'class="active"'; ?>><a href="/api">API</a></li>
                <li <? if ($data['page'] == 'faq') echo 'class="active"'; ?>><a href="/faq">FAQ</a></li>
            </ul>
        </nav>
    </div>
</div>


<div class="inner cover">
