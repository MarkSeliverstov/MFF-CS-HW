<?php

class Controller_Article extends Controller
{
	
    function __construct()
	{
		$this->model = new Model_Articles();
		$this->view = new View();
	}

	function action_index($id)
	{
        if ($id == null){
            throw new Exception('Controller not found');
        }
        $data = $this->model->get_article($id);
		$this->view->generate('article_view.php', $data);
	}

    function action_edit($id){
        if ($id == null){
            throw new Exception('Controller not found');
        }
        $data = $this->model->get_article($id);
		$this->view->generate('article-edit_view.php', $data);
    }
}
