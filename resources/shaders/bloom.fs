#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D scene;
uniform sampler2D bloomBlur;
uniform sampler2D msaa;
uniform bool bloom;
uniform float exposure;

void main()
{
    const float gamma = 1.2;
    vec3 hdrColor = vec3(0.0);

    // Resolve MSAA texture
    vec3 msaaColor = vec3(0.0);
    for (int i = 0; i < 8; i++) {
        vec4 sample = texture(msaa, TexCoords, i);
        msaaColor += sample.rgb;
    }
    msaaColor /= 8.0;

    hdrColor = msaaColor;

    vec3 bloomColor = texture(bloomBlur, TexCoords).rgb;
    if(bloom)
        hdrColor += bloomColor; // additive blending

    // tone mapping
    vec3 result = vec3(1.0) - exp(-hdrColor * exposure);
    // also gamma correct while we're at it
    result = pow(result, vec3(1.0 / gamma));

    FragColor = vec4(result, 1.0);
}

