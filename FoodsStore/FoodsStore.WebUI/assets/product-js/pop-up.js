/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

var openLogin = false;
var openSignup = false;

function closeSignup() {
    document.getElementById('signup-form').style.display = 'none';
    openSignup = false;
}
function displaySignup() {
    if (openLogin) {
        closeLogin();
    }
    document.getElementById('signup-form').style.display = 'block';
    openSignup = true;
}
function closeLogin() {
    document.getElementById('login-form').style.display = 'none';
    openLogin = false;
}
function displayLogin() {
    if (openSignup) {
        closeSignup();
    }
    document.getElementById('login-form').style.display = 'block';
    openLogin = true;
}

function fromLoginToSignup() {
    closeLogin();
    displaySignup();
}
function fromSignupToLogin() {
    closeSignup();
    displayLogin();
}

function displayUserCommentBox() {
    document.getElementById('user-comment').style.display = 'block';
    document.getElementById('text-user-comment').focus();
}
function clickTextComment() {
    document.getElementById('text-user-comment').
            document.getElementById('text-user-comment').focus();
}

function closeFormMessage() {
    document.getElementById('form-message').style.display = 'none';
}

function AvoidSpace(event) {
    var k = event ? event.which : window.event.keyCode;
    if (k === 32)
        return false;
}
