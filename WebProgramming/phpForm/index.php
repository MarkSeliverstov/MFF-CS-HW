<?php

require_once(__DIR__ . '/recodex_lib.php');

if ($_SERVER['REQUEST_METHOD'] == 'POST'){
    $message = 'ERROR';
    $invalidFields = [];

    // validate firstname
    if (isset($_POST['firstName']) && 
        !empty($_POST['firstName']) && 
        mb_strlen($_POST['firstName']) < 100
        ){
        $firstName = $_POST['firstName'];
    }
    else{
        $invalidFields[] = 'firstName';
    }
    
    // validate lasttname
    if (isset($_POST['lastName']) && 
        !empty($_POST['lastName']) &&
        mb_strlen($_POST['lastName']) < 100
        ){
        $lastName = $_POST['lastName'];
    }
    else{
        $invalidFields[] = 'lastName';
    }

    // validate email
    if (isset($_POST['email']) && 
        !empty($_POST['email']) && 
        filter_var($email, FILTER_VALIDATE_EMAIL) &&
        mb_strlen($_POST['email']) < 200
        ){
        $email = $_POST['email'];
    }
    else{    
        $invalidFields[] = 'email';
    }

    // validate deliveryBoy
    if (isset($_POST['deliveryBoy']) && (
        $_POST['deliveryBoy'] === 'jesus' ||
        $_POST['deliveryBoy'] === 'santa' ||
        $_POST['deliveryBoy'] === 'moroz' ||
        $_POST['deliveryBoy'] === 'hogfather' ||
        $_POST['deliveryBoy'] === 'czpost' ||
        $_POST['deliveryBoy'] === 'fedex'
    )){
        $deliveryBoy = $_POST['deliveryBoy'];
    }
    else{
        $invalidFields[] = 'deliveryBoy';
    }

    // validate unboxDay
    if (isset($_POST['unboxDay']) && 
        filter_var($num, FILTER_VALIDATE_INT) && (
        $_POST['unboxDay'] === 24 ||
        $_POST['unboxDay'] === 25
    )){
        $unboxDay = $_POST['unboxDay'];
    }
    else{
        $invalidFields[] = 'unboxDay';
    }
    
    // validate fromTime and toTime             TODO:
    $fromTime = null;
    $toTime = null;
    if (isset($_POST['fromTime'])){

    }
    else{
        $invalidFields[] = 'fromTime';
    }
    if (isset($_POST['toTime'])){

    }
    else{
        $invalidFields[] = 'toTime';
    }

    //validate gifts
    $gifts = [];
    $giftCustom = null;
    if (isset($_POST['gifts[]'])){
        if (in_array('socks', $_POST['gifts[]']) ||
            in_array('points', $_POST['gifts[]']) ||
            in_array('jarnik', $_POST['gifts[]']) ||
            in_array('cash', $_POST['gifts[]']) ||
            in_array('teddy', $_POST['gifts[]'])
            ){
            $gifts = $_POST['gifts[]'];
        }
        if (in_array('other', $_POST['gifts[]']) && 
            !empty($_POST['gifts[]']['other'])
        ){
            $giftCustom = $_POST['gifts[]']['other'];
        }
        else{
            $invalidFields[] = 'gifts[]';
        }
    }




    if (!empty($invalidFields))
        recodex_survey_error($message, $invalidFields);
    else{
        recodex_save_survey(
            $firstName, 
            $lastName, 
            $email, 
            $deliveryBoy,
            $unboxDay,
            $fromTime,
            $toTime,
            $gifts,
            $giftCustom,
        );
    }
    header('index.php', false, 302);
}
if ($_SERVER['REQUEST_METHOD'] == 'GET'){
    require __DIR__ . '/form_template.html';
}
