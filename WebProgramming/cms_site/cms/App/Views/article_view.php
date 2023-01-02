<script type="text/javascript">
    function edit(){
        window.location = 'https://webik.ms.mff.cuni.cz/~13176152/cms/article-edit/<?php echo $data[0]['id']?>'
    }

    function back(){
        window.location = 'https://webik.ms.mff.cuni.cz/~13176152/cms/articles'
    }
</script>

<div class="content">
    <div class="paper">
        <h1><?php echo $data[0]['title']?></h1>
        <pre style="white-space: pre-wrap">
            <p class="article__content"><?php echo $data[0]['content']?></p>
        </pre>
        <div class="article__show__btns">
            <button class="btn edit" onclick="edit()" is="edit_btn">Edit</button>
            <button class="btn" onclick="back()" is="edit_btn">Back to articles</button>
        </div>
    </div>
</div>
