<?php
// bootstrap.php

use Doctrine\DBAL\DriverManager;
use Doctrine\ORM\EntityManager;
use Doctrine\ORM\ORMSetup;

$config=ORMSetup::createAttributeMetadataConfiguration(
  paths: array(__DIR__."/src"),
  isDevMode: true,
);

$connectionParams = [
    'dbname' => 'stud_13176152',
    'user' => '13176152',
    'password' => 'mLLPUyOR',
    'host' => 'webik.ms.mff.cuni.cz',
    'driver' => 'mysqli',
];

$conn = DriverManager::getConnection($connectionParams);

$entityManager = new EntityManager($connection, $config);

$productRepository = $entityManager->getRepository('custom\Article');

$articles = $productRepository->findAll();
foreach ($articles as $article) {
  echo "<br>";
  var_dump($article);
}