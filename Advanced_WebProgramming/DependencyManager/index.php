<?php
# Allow for autoloading
require __DIR__ . '/vendor/autoload.php';

$log = new Monolog\Logger('name');

$log->pushHandler(new Monolog\Handler\StreamHandler(__DIR__ . '/data/application.log', Monolog\Logger::INFO));

$log->Debug('Page is loading');

 