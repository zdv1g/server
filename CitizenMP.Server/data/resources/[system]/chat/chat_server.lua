RegisterServerEvent('chatCommandEntered')
RegisterServerEvent('chatMessageEntered')

AddEventHandler('chatMessageEntered', function(name, color, message)
    if not name or not color or not message or #color ~= 3 then
        return
    end
	
	while string.find(message, '^%s') do
		message = string.sub(message, 2)
	end
	
	while string.find(message, '%s$') do
		message = string.sub(message, 1, string.len(message) - 1)
	end
	
	if message ~= '' then
		if string.find(message, '/') == 1 then
			local command
			local params = {}
			if string.find(message, '%s') ~= nil then
				command = string.sub(message, 2, string.find(message, '%s') - 1)
				message = string.sub(message, string.find(message, '%s') + 1)
				
				while true do
					if string.find(message, '%s') == nil then
						table.insert(params, message)
						break
					elseif string.find(message, '%s') == string.len(message) then
						table.insert(params, string.sub(message, 1, -2))
						break
					else
						local param = string.sub(message, 1, string.find(message, '%s') - 1)
						table.insert(params, param)
						message = string.sub(message, string.find(message, '%s') + 1)
					end
				end
			else
				command = string.sub(message, 2)
			end
			
			TriggerEvent('commandEntered', source, command, params)
		else
			TriggerEvent('chatMessage', source, name, message)

			if not WasEventCanceled() then
				TriggerClientEvent('chatMessage', -1, name, color, message)
			end

			print(name .. ': ' .. message)
		end
	end
	
	
end)

-- player join messages
AddEventHandler('playerActivated', function()
    TriggerClientEvent('chatMessage', -1, '', { 0, 0, 0 }, '^2* ' .. GetPlayerName(source) .. ' joined.')
	print('^2* ' .. GetPlayerName(source) .. ' joined.')
end)

AddEventHandler('playerDropped', function(reason)
    TriggerClientEvent('chatMessage', -1, '', { 0, 0, 0 }, '^2* ' .. GetPlayerName(source) ..' left (' .. reason .. ')')
	print('^2* ' .. GetPlayerName(source) .. ' left (' .. reason .. ')')
end)

-- say command handler
AddEventHandler('rconCommand', function(commandName, args)
    if commandName == "say" then
        local msg = table.concat(args, ' ')

        TriggerClientEvent('chatMessage', -1, 'console', { 0, 0x99, 255 }, msg)
        RconPrint('console: ' .. msg .. "\n")

        CancelEvent()
    end
end)

-- tell command handler
AddEventHandler('rconCommand', function(commandName, args)
    if commandName == "tell" then
        local target = table.remove(args, 1)
        local msg = table.concat(args, ' ')

        TriggerClientEvent('chatMessage', tonumber(target), 'console', { 0, 0x99, 255 }, msg)
        RconPrint('console: ' .. msg .. "\n")

        CancelEvent()
    end
end)
