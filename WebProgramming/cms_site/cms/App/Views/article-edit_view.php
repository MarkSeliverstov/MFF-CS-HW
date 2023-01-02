<script type="text/javascript">
    function save(){
        let content = document.getElementById('edit__text').value.replace(/\n/g, '<br/>');
        console.log(content);
        window.location = 'https://webik.ms.mff.cuni.cz/~13176152/cms/article-save/<?php echo $data[0]['id']?>?content=' + content;
    }

    function back(){
        window.location = 'https://webik.ms.mff.cuni.cz/~13176152/cms/articles'
    }
</script>

<div class="content">
    <div class="paper">
        <form class="edit_form" action="" name="Name">
            <h1>Name</h1>
            <input maxlength="32" class="edit__name" type="text" value="<?php echo $data[0]['title']?>">
            <h1 class="h1__edit__content">Content</h1>
            <pre style="white-space: pre-wrap">
                <textarea class="edit__text" id="edit__text" maxlength="1024" value="<?php echo $data[0]['content']?>" mmaxlength="20" class="input_edit"></textarea>
            </pre>
        </form>
        <div class="article__show__btns">
            <button class="btn save" id="save" onclick="save()">Save</button>
            <button class="btn return" onclick="back()">Back to articles</button>
        </div>
    </div>
</div>