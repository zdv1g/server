RegisterNetEvent('spawnCar')
RegisterNetEvent('setSkin')
RegisterNetEvent('teleport')
RegisterNetEvent('giveWeapon')

AddEventHandler('spawnCar', function(car)
	Citizen.CreateThread(function()
		Citizen.Wait(0)
		local model = GetHashKey(car)
		if IsModelInCdimage(model) and IsModelAVehicle(model) then
			local playerPed = GetPlayerPed(-1)
			if playerPed and playerPed ~= -1 then
				RequestModel(model)
				
				while not HasModelLoaded(model) do
					Citizen.Wait(0)
				end
				
				local coords = GetOffsetFromEntityInWorldCoords(playerPed, 0, 5.0, 0)
				CreateVehicle(model, coords, 0.0, true, false)
				
				TriggerEvent('chatMessage', '', {0, 0, 0}, 'Spawned '..skin)
				
				Wait(100)
				SetModelAsNoLongerNeeded(model)
			end
		else
			TriggerEvent('chatMessage', '', {0, 0, 0}, 'Model not found')
		end
	end)
end)

AddEventHandler('setSkin', function(skin)
	Citizen.CreateThread(function()
		Citizen.Wait(0)
		local model = GetHashKey(skin)
		if IsModelInCdimage(model) and IsModelValid(model) then
			local playerPed = GetPlayerPed(-1)
			if playerPed and playerPed ~= -1 then
				RequestModel(model)
				
				while not HasModelLoaded(model) do
					Citizen.Wait(0)
				end
				
				local veh = false
				if IsPedInAnyVehicle then
					veh = GetVehiclePedIsUsing(playerPed)
				end
				
				SetPlayerModel(PlayerId(), model)
				SetPedDefaultComponentVariation(playerPed)
				Wait(0)
				
				if veh then
					SetPedIntoVehicle(playerPed, veh, -1)
				end
				
				TriggerEvent('chatMessage', '', {0, 0, 0}, 'Changed skin to '..skin)
				
				Wait(100)
				SetModelAsNoLongerNeeded(model)
			end
		else
			TriggerEvent('chatMessage', '', {0, 0, 0}, 'Model not found')
		end
	end)
end)

AddEventHandler('teleport', function(target)
	local targetId = tonumber(target)
	if not NetworkIsPlayerConnected(targetId) then
		TriggerEvent('chatMessage', '', {0, 0, 0}, 'Player not found')
	elseif DoesEntityExist(GetPlayerPed(targetId)) then
		local us = PlayerPedId()
		
		if IsPedInAnyVehicle(us, 0) then
			local veh = GetVehiclePedIsUsing(us)
			if GetPedInVehicleSeat(veh, -1) == us then
				us = veh;
			end
		end
		
		local targetpos = GetEntityCoords(GetPlayerPed(targetId), 0)
		
		local targetCoords = {}
		
		targetCoords.x = targetpos.x + 3
		targetCoords.y = targetpos.y + 3
		targetCoords.z = targetpos.z
		
		SetEntityCoordsNoOffset(us, targetCoords.x, targetCoords.y, targetCoords.z, 0, 0, 1)

		TriggerEvent('chatMessage', '', {0, 0, 0}, 'Teleported to '..GetPlayerName(targetId))
	else
		TriggerEvent('chatMessage', '', {0, 0, 0}, 'Player does not exist')
	end
end)

AddEventHandler('giveWeapon', function(weapon)
	local player = PlayerId()
	local playerPed = PlayerPedId()
	local playerExists = DoesEntityExist(playerPed)
	local model = GetHashKey(weapon)

	if playerExists then
		GiveDelayedWeaponToPed(playerPed, model, 1000, true)
	end
end)
