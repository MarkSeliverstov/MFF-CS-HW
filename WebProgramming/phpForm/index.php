<?php

//require_once(__DIR__ . '/recodex_lib.php');
if (!empty($_POST)){
    echo '<pre>' . print_r($_POST, 1) . '</pre>';
    // foreach($_POST as $key => $value){
    //     if(!array_key_exists($key, $_POST)){
    //         recodex_survey_error();
    //         return;
    //     }
    // }
}



// if (!empty($_POST)){
//     recodex_save_survey(
//         $_POST['firstName'],
//         $_POST['lastName'],
//         $_POST['email'],
//         $_POST['deliveryBoy'],		// enum (jesus|santa|moroz|hogfather|czpost|fedex)
//         $_POST['unboxDay'],				// 24 or 25
//         $_POST['fromTime'],		// minutes from midnight or null
//         $_POST['toTime'],			// minutes from midnight or null
//         $_POST['gifts'],			// array of enum strings (socks|points|jarnik|cash|teddy|other)
//         $_POST['giftCustom']);
// }
require __DIR__ . '/form_template.html';
