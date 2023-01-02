<div class="content">
    <div>
        <h1>Article list</h1>
    </div>
    <hr>
    <div id="article__list">
        <?php foreach ($data as $article){ ?>

        <div class="article" id="<?php echo $article['id'];?>">
            <div class="article__title">
                <p class="article__title__list"><?php echo $article['title']; ?></p>
            </div>
            <div class="article__control">
                <a class="show__btn" href="article/<?php echo $article['id']; ?>">Show</a>
                <a class="edit__btn" href="article-edit/<?php echo $article['id']; ?>">Edit</a>
                <button class="delete" id="delete">Delete</button>
            </div>
        </div>

        <?php } ?>
    </div>
    <hr>
    <div class="articles__btns">
        <div class="control__pages">
            <button class="previous btn" id="previous__page">Previous</button>
            <p id="count__of__pages"></p>
            <button class="next btn" id="next__page">Next</button>
        </div>
        <button class="btn create" id="openDialogButton">Create article</button>
    </div>
</div>


<dialog id="article__dialog">
    <p>Enter a name of article</p>
    <form name="article__form" class="form">
        <input style="font-size: 26px;" class="form__input" id="form__input" type="text" value="" maxlength="32" required>
        <div class="dialog__btns">
            <input class="dialog_cancele_btn" id="closeDialog" type="button" value="Cancel">
            <input class="submit" id="submit" type="button" value="Create">
        </div>
    </form>
</dialog>

<script type="text/javascript" src="lib/js/article_list.js"></script>
