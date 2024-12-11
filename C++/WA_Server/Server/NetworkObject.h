#pragma once

class NetworkObject
{
public:
	NetworkObject() {

	}
	virtual ~NetworkObject() {

	}
public:
	virtual void Update() abstract;
};
