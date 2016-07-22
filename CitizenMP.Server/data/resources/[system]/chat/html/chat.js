function colorize(string)
{
    var newString = '';
    var inSpan = false;

    for (i = 0; i < string.length; i++)
    {
        if (string[i] == '^')
        {
            if (string[i + 1] == '7' || string[i + 1] == '0')
            {
                if (inSpan)
                {
                    newString += '</span>';

                    inSpan = false;
                }

                i += 2;
            }
            else if (string[i + 1] >= '0' && string[i + 1] <= '9')
            {
                if (inSpan)
                {
                    newString += '</span>';
                }

                i += 2;
                newString += '<span class="color-' + string[i - 1] + '">';

                inSpan = true;
            }
        }

        newString += string[i];
    }

    if (inSpan)
    {
        newString += '</span>';
    }

    return newString;
}

$(function()
{
    var chatHideTimeout;
    var inputShown = false;
	
	var bufCounter = 0;
	var buffer = new Array();

    function startHideChat()
    {
        if (chatHideTimeout)
        {
            clearTimeout(chatHideTimeout);
        }

        if (inputShown)
        {
            return;
        }

        chatHideTimeout = setTimeout(function()
        {
            if (inputShown)
            {
                return;
            }

            $('#chat').animate({ opacity: 0 }, 300);
        }, 7000);
    }

    handleResult = function(elem, wasEnter)
    {
        inputShown = false;

        $('#chatInputHas').hide();

        startHideChat();

        var obj = {};

        if (wasEnter)
        {
            obj = { message: $(elem).val() };

			buffer.push($(elem).val());
			bufCounter = buffer.length;
        }

        $(elem).val('');

        $.post('http://chat/chatResult', JSON.stringify(obj), function(data)
        {
            console.log(data);
        });
    };

    $('#chatInput').fakeTextbox(); // //

    $('#chatInput')[0].onPress(function(e)
    {
        if (e.which == 13)
        {
            handleResult(this, true);
        }
    });

    $(document).keyup(function(e)
    {
        if (e.keyCode == 27)
        {
            handleResult($('#chatInput')[0].getTextBox(), false);
        }
    });

    $(document).keypress(function(e)
    {
        if (e.keyCode == 9)
        {
            e.preventDefault();
            return false;
        }
    });
	
	function $log(str) {
		var data = {
			color: [0, 0, 0],
			name: 'Server',
			message: ''+str
		}
		var event = new Event('message')
		event.data = data
		window.dispatchEvent(event)
	}

    window.addEventListener('message', function(event)
    {
        var item = event.data;

        if (item.meta && item.meta == 'openChatBox')
        {
            inputShown = true;

            $('#chat').css('opacity', '1');
			
            $('#chatInputHas').show();
            $('#chatInput')[0].doFocus();

            return;
        }

        // TODO: use some templating stuff for this
        var colorR = parseInt(item.color[0]);
        var colorG = parseInt(item.color[1]);
        var colorB = parseInt(item.color[2]);

        var name = item.name.replace('<', '&lt;');
        var message = item.message.replace('<', '&lt;');

        message = colorize(message);

        var buf = $('#chatBuffer');

        var nameStr = '';

        if (name != '')
        {
            nameStr = '<strong style="color: rgb(' + colorR + ', ' + colorG + ', ' + colorB + ')">' + name + ': </strong>';
        }

        buf.find('ul').append('<li>' + nameStr + message + '</li>');
        buf.scrollTop(buf[0].scrollHeight - buf.height());

        $('#chat').css('opacity', '1');

        startHideChat();
    }, false);
	
	var layout = 0;
	var counter = 0;
	var latText = '';
	
	var shiftPressed = false;
	var ctrlPressed = false;
	var changed = false;
	
	var letters = new Array();
	
	letters[0] = new Array();
	letters[1] = new Array();
	
	letters[0]['й'] = 'q';
	letters[0]['Й'] = 'Q';
	letters[0]['ц'] = 'w';
	letters[0]['Ц'] = 'W';
	letters[0]['у'] = 'e';
	letters[0]['У'] = 'E';
	letters[0]['к'] = 'r';
	letters[0]['К'] = 'R';
	letters[0]['е'] = 't';
	letters[0]['Е'] = 'T';
	letters[0]['н'] = 'y';
	letters[0]['Н'] = 'Y';
	letters[0]['г'] = 'u';
	letters[0]['Г'] = 'U';
	letters[0]['ш'] = 'i';
	letters[0]['Ш'] = 'I';
	letters[0]['щ'] = 'o';
	letters[0]['Щ'] = 'O';
	letters[0]['з'] = 'p';
	letters[0]['З'] = 'P';
	letters[0]['х'] = '[';
	letters[0]['Х'] = '{';
	letters[0]['ъ'] = ']';
	letters[0]['Ъ'] = '}';
	letters[0]['ф'] = 'a';
	letters[0]['Ф'] = 'A';
	letters[0]['ы'] = 's';
	letters[0]['Ы'] = 'S';
	letters[0]['в'] = 'd';
	letters[0]['В'] = 'D';
	letters[0]['а'] = 'f';
	letters[0]['А'] = 'F';
	letters[0]['п'] = 'g';
	letters[0]['П'] = 'G';
	letters[0]['р'] = 'h';
	letters[0]['Р'] = 'H';
	letters[0]['о'] = 'j';
	letters[0]['О'] = 'J';
	letters[0]['л'] = 'k';
	letters[0]['Л'] = 'K';
	letters[0]['д'] = 'l';
	letters[0]['Д'] = 'L';
	letters[0]['ж'] = ';';
	letters[0]['Ж'] = ':';
	letters[0]['э'] = '\'';
	letters[0]['Э'] = '"';
	//letters[0]['/'] = '|';
	letters[0]['я'] = 'z';
	letters[0]['Я'] = 'Z';
	letters[0]['ч'] = 'x';
	letters[0]['Ч'] = 'X';
	letters[0]['с'] = 'c';
	letters[0]['С'] = 'C';
	letters[0]['м'] = 'v';
	letters[0]['М'] = 'V';
	letters[0]['и'] = 'b';
	letters[0]['И'] = 'B';
	letters[0]['т'] = 'n';
	letters[0]['Т'] = 'N';
	letters[0]['ь'] = 'm';
	letters[0]['Ь'] = 'M';
	letters[0]['б'] = ',';
	letters[0]['Б'] = '<';
	letters[0]['ю'] = '.';
	letters[0]['Ю'] = '>';
	letters[0]['.'] = '/';
	letters[0][','] = '?';
	letters[0]['ё'] = '`';
	letters[0]['Ё'] = '~';
	letters[0]['"'] = '@';
	letters[0]['№'] = '#';
	letters[0][';'] = '$';
	letters[0][':'] = '^';
	//letters[0]['?'] = '&';
	
	letters[1]['q'] = 'й';
	letters[1]['Q'] = 'Й';
	letters[1]['w'] = 'ц';
	letters[1]['W'] = 'Ц';
	letters[1]['e'] = 'у';
	letters[1]['E'] = 'У';
	letters[1]['r'] = 'к';
	letters[1]['R'] = 'К';
	letters[1]['t'] = 'е';
	letters[1]['T'] = 'Е';
	letters[1]['y'] = 'н';
	letters[1]['Y'] = 'Н';
	letters[1]['u'] = 'г';
	letters[1]['U'] = 'Г';
	letters[1]['i'] = 'ш';
	letters[1]['I'] = 'Ш';
	letters[1]['o'] = 'щ';
	letters[1]['O'] = 'Щ';
	letters[1]['p'] = 'з';
	letters[1]['P'] = 'З';
	letters[1]['['] = 'х';
	letters[1]['{'] = 'Х';
	letters[1][']'] = 'ъ';
	letters[1]['}'] = 'Ъ';
	letters[1]['a'] = 'ф';
	letters[1]['A'] = 'Ф';
	letters[1]['s'] = 'ы';
	letters[1]['S'] = 'Ы';
	letters[1]['d'] = 'в';
	letters[1]['D'] = 'В';
	letters[1]['f'] = 'а';
	letters[1]['F'] = 'А';
	letters[1]['g'] = 'п';
	letters[1]['G'] = 'П';
	letters[1]['h'] = 'р';
	letters[1]['H'] = 'Р';
	letters[1]['j'] = 'о';
	letters[1]['J'] = 'О';
	letters[1]['k'] = 'л';
	letters[1]['K'] = 'Л';
	letters[1]['l'] = 'д';
	letters[1]['L'] = 'Д';
	letters[1][';'] = 'ж';
	letters[1][':'] = 'Ж';
	letters[1]['\''] = 'э';
	letters[1]['"'] = 'Э';
	letters[1]['|'] = '/';
	letters[1]['z'] = 'я';
	letters[1]['Z'] = 'Я';
	letters[1]['x'] = 'ч';
	letters[1]['X'] = 'Ч';
	letters[1]['c'] = 'с';
	letters[1]['C'] = 'С';
	letters[1]['v'] = 'м';
	letters[1]['V'] = 'М';
	letters[1]['b'] = 'и';
	letters[1]['B'] = 'И';
	letters[1]['n'] = 'т';
	letters[1]['N'] = 'Т';
	letters[1]['m'] = 'ь';
	letters[1]['M'] = 'Ь';
	letters[1][','] = 'б';
	letters[1]['<'] = 'Б';
	letters[1]['.'] = 'ю';
	letters[1]['>'] = 'Ю';
	letters[1]['/'] = '.';
	letters[1]['?'] = ',';
	letters[1]['`'] = 'ё';
	letters[1]['~'] = 'Ё';
	letters[1]['@'] = '"';
	letters[1]['#'] = '№';
	letters[1]['$'] = ';';
	letters[1]['^'] = ':';
	letters[1]['&'] = '?';
	
	setInterval(function() {
		var text = $('#text-sync').val()
		if(counter < text.length) {
			if(text.substr(counter, 1) == 'й') console.log(text[counter][0]);
			if(letters[layout][text.substr(counter, 1)]) {
				newText = text.substr(0, counter)+letters[layout][text.substr(counter, 1)]+text.substr(counter+2)
				text[counter] = letters[layout][text.substr(counter, 1)];
				$('#text-sync').val(newText);
				//$log(text[counter]+' => '+letters[layout][text.substr(counter, 1)]);
			}
			counter++;
		} else if(counter > text.length){
			counter = text.length;
		}
	}, 1)
	
	window.onkeydown = window.onkeyup = function(e) {
		if(e.keyCode == 16) {
			if(e.type == 'keydown'){
				shiftPressed = true;
			} else if(e.type == 'keyup') {
				changed = false
				shiftPressed = false;
			}
		}
		if(e.keyCode == 17) {
			if(e.type == 'keydown'){
				ctrlPressed = true;
			} else if(e.type == 'keyup') {
				changed = false
				ctrlPressed = false;
			}
		}
		if (e.keyCode == 18) {
			if(e.type == 'keyup'&&shiftPressed){
				if(layout == 1) layout = 0;
				else layout = 1;
				$('#chatInputHas strong')[0].innerHTML = 'Chat ' + ((layout == 0)?'[EN]':'[RU]');
			}
		}
		if(shiftPressed&&ctrlPressed&&(!changed)) {
			if(layout == 1) layout = 0;
			else layout = 1;
			changed = true;
			$('#chatInputHas strong')[0].innerHTML = 'Chat ' + ((layout == 0)?'[EN]':'[RU]');
		}
		if(e.keyCode == 38) {
			e.preventDefault();
			if(e.type == 'keyup'&&bufCounter>0){
				$('#text-sync').val(buffer[bufCounter-1])
				counter = $('#text-sync').val().length;
				$(document).trigger('updatetb');
				bufCounter--;
			}
		}
		if(e.keyCode == 40) {
			e.preventDefault();
			if(e.type == 'keyup'&&bufCounter<buffer.length){
				$('#text-sync').val(buffer[bufCounter+1])
				counter = $('#text-sync').val().length;
				$(document).trigger('updatetb');
				bufCounter++;
			}
		}
	}
});
