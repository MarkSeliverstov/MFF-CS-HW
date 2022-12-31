<div class="article__list">
    <div>
        <h1>Article list</h1>
    </div>
    <hr>
    <div>
        <?php foreach ($data as $article){ ?>
        <div>
            <p class="article__title__list"><?php echo $article['title']; ?></p>
            <a class="show__btm" href="/article/<?php echo $article['id']; ?>">Show</a>
            <a class="edit__btm" href="/article-edit/<?php echo $article['id']; ?>">Edit</a>
            <a class="delete__btm" href="/article-edit/<?php echo $article['id']; ?>">Delete</a>
        </div>
        <?php } ?>
    </div>
    <hr>
    <div>
        <button class="previous__btm" >Previous</button>
        <button class="next__btm">Next</button>
        <p class="count__of__pages">page count: </p>
        <button class="create__btm">Create article</button>
    </div>
</div>

