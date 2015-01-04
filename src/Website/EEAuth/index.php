<?php include 'app/simple.php';
$data['page'] = '';

SimplePhp::DefaultPage(0, 'index');               # Redirect example.com to example.com/index
SimplePhp::NotFoundPage('controllers' , 0, 'notfound'); # If no view is found, display the /notfound page

SimplePhp::LoadDir('controllers', 0);             # Run the controller
SimplePhp::LoadPage('templates', 'header');       # Display the header
SimplePhp::LoadDir('views', 0);                   # Display the content
SimplePhp::LoadPage('templates', 'footer');       # Display the footer