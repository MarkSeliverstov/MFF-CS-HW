<?php

class Controller_Articles extends Controller
{

	function __construct()
	{
		$this->model = new Model_Articles();
		$this->view = new View();
	}
	
	function action_index($id = null)
	{
        if ($id != null){
            throw new Exception('Controller not found');
        }
		$data = $this->model->get_article_list();		
		$this->view->generate('articles_view.php', $data);
	}
}
