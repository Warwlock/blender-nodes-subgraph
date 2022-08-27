float GBackFacing(float3 _POS, float3 normal)
{
	float3 worldDx = ddx(_POS);
	float3 worldDy = ddy(_POS);

	float3 crossX = cross(worldDy, normal);

	float det = dot(worldDx, crossX);

	return sign(det);
}