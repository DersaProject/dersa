<?php
$file = 'bin/bak/Dersa.Common.dll';
 
if (copy($file, 'bin/Dersa.Common.dll')) {
    echo $file;
    echo " was copied!";
}else{
    echo $file;
    echo " Failed to copy ";
}
?>