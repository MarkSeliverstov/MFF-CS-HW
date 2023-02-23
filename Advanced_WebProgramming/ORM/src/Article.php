<?php
use Doctrine\ORM\Mapping as ORM;

/**
 * @ORM\Entity
 * @ORM\Table(name="Articles")
 */
class Articles
{
    /** 
     * @ORM\Column(type="integer") 
     * @ORM\Id
     */
    private int $id;
    /** 
     * @ORM\Column(type="string") 
     */
    private string $name;
}
