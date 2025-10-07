// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Simple connection loss handler to cache last answer payload
window.QuizzlyCache = (function(){
    const KEY = 'quizzly_temp_answer';
    return {
        save(payload){ try{ localStorage.setItem(KEY, JSON.stringify(payload)); }catch(e){} },
        read(){ try{ const v = localStorage.getItem(KEY); return v? JSON.parse(v): null;}catch(e){ return null;} },
        clear(){ try{ localStorage.removeItem(KEY);}catch(e){} }
    }
})();