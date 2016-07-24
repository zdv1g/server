AddEventHandler('commandEntered', function(src, cmd, params)
	if cmd == 'spawn' then
		if params[1] then
			TriggerClientEvent('spawnCar', src, params[1])
		else
			TriggerClientEvent('chatMessage', src, '', { 0, 0, 0 }, 'Usage: /spawn [car]')
		end
	elseif cmd == 'skin' then
		if params[1] then
			TriggerClientEvent('setSkin', src, params[1])
		else
			TriggerClientEvent('chatMessage', src, '', { 0, 0, 0 }, 'Usage: /skin [skin]')
		end
	elseif cmd == 'tp' or cmd == 'teleport' then
		if params[1] then
			TriggerClientEvent('teleport', src, params[1])
		else
			TriggerClientEvent('chatMessage', src, '', { 0, 0, 0 }, 'Usage: /tp [playerId]')
		end
	elseif cmd == 'weapon' then
		if params[1] then
			TriggerClientEvent('giveWeapon', src, params[1])
		else
			TriggerClientEvent('chatMessage', src, '', { 0, 0, 0 }, 'Usage: /weapon [weapon]')
		end
	end
end)