<?php

class Controller {
	
	public $model;
	public $view;
	
	function __construct()
	{
		$this->view = new View();
	}
	
	// defoult action
	function action_index($id)
	{
        // todo
	}
}
