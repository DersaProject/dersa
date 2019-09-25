<?php
$file = 'bin/bak/Dersa.Stereotypes.dll';
 
if (copy($file, 'bin/Dersa.Stereotypes.dll')) {
    echo "File was copied!";
}else{
    echo $file;
    echo " Failed to copy file";
}
?>