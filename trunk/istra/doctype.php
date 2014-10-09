<?php
function setDocType($html){
	$res = preg_replace('/<!DOCTYPE[^>]*>/', '<!DOCTYPE html>', $html);
	$res = preg_replace('/<html[^>]*>/i', '<html>', $res);
	return $res;
}