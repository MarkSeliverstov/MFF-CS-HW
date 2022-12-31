<?php

class Model_Articles extends Model
{
	
	public function get_data($id = null)
	{
        return array (
            array(
                'id' => 1,
                'title' => 'Article title',
                'content' => 'Article content',
            ),
        );
	}

    public function get_article($id){
        return $id;
    }

}
