<?php

class View
{
	
	public $template_view = "template_view.php";

	function generate($content_view, $data = null)
	{
		
		/*
		if(is_array($data)) {
            
		}
		*/

		include __DIR__. '/../views/'. $this->template_view;
	}
}
