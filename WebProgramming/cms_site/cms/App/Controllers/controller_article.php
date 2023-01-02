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

    function action_create($name){
        $id = $this->model->create_article($name);
        $host = 'http://'.$_SERVER['HTTP_HOST'].'/';
        header('Location:'.$host.'/~13176152/cms/article-edit/'.$id);
    }

    function action_save($id, $content){
        $this->model->save_edited_article($id, $content);
        $host = 'http://'.$_SERVER['HTTP_HOST'].'/';
        header('Location:'.$host.'/~13176152/cms/articles');
    }

    function action_delete($id)
	{
        $this->model->delete_article($id);
	}
}
