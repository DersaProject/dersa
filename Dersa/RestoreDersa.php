<?php
$file = '../bin/bak/Dersa.dll';
 
if (copy($file, '../bin/Dersa.dll')) {
    echo $file;
    echo " was copied!";
}else{
    echo $file;
    echo " Failed to copy ";
}
?>