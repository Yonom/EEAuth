<?php $path = array_slice(explode('/', strtok($_SERVER["REQUEST_URI"], '?')), 1); $data = array();

class SimplePhp {
    static function DefaultPage($segment, $defaultPage) {
        global $path;
        if (!$path[$segment])
            $path[$segment] = $defaultPage;
    }

    static function NotFoundPage($dir, $segment, $notfoundPage) {
        global $path;
        if (!file_exists(SimplePhp::GetFile($dir, $path[$segment])))
            $path[$segment] = $notfoundPage;
    }

    static function LoadDir($dir, $segment) {
        global $path;
        SimplePhp::IncludePage(SimplePhp::GetFile($dir, $path[$segment]));
    }

    static function LoadPage($dir, $page) {
        SimplePhp::IncludePage(SimplePhp::GetFile($dir, $page));
    }

    private static function IncludePage($page) {
        global $path, $data;
        if (file_exists($page))
            include($page);
    }

    private static function GetFile($dir, $page) {
        return SimplePhp::GetPath($dir) . $page .'.php';
    }

    private static function GetPath($dir) {
        return dirname(__FILE__) . '/' . $dir . '/';
    }
}