void node_bump(float3 _POS, float invert, float strength, float dist, float height, float3 normal, out float4 normalOut)
{
	float3 worldDx = ddx(_POS);
	float3 worldDy = ddy(_POS);

	float3 crossX = cross(worldDy, normal);
	float3 crossY = cross(normal, worldDx);

	float det = dot(worldDx, crossX);
	dist *= det < 0 ? invert : -invert;

	float3 dHdx = ddx(height);
	float3 dHdy = ddy(height);

	float3 surfgrad = dHdx * crossX + dHdy * crossY;

	strength = max(strength, 0);

	normalOut = float4(normalize(abs(det) * normal - dist * sign(det) * surfgrad), 0);
	normalOut = float4(normalize(lerp(normal, normalOut, strength)), 0);
}