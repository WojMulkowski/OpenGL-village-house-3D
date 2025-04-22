#version 330

uniform mat4 P;
uniform mat4 V;
uniform mat4 M;
uniform sampler2D tex;

out vec4 pixelColor; //Zmienna wyjsciowa fragment shadera. Zapisuje sie do niej ostateczny (prawie) kolor piksela

in vec2 i_tc;
in vec4 i_c;
in vec4 i_n;
in vec4 i_v;

void main(void) {   
    
    vec4 color = texture2D(tex,i_tc);

	vec4 polZroSwi = vec4(0, 0, -6, 1);
    vec4 l = normalize(V*polZroSwi-V*M*i_v);
    vec4 n = normalize(V*M*i_n);
    vec4 r = reflect(-l,n);
    

    vec4 v = normalize(vec4(0,0,0,1)-V*M*i_v);
    float rv = clamp(dot(r,v),0,1);
    rv = pow(rv,100);
    
    pixelColor = color*vec4(0, 0, 0, 1)+color*vec4(1, 1, 1, 1)*dot(n, l)+vec4(1, 1, 1, 1)*rv;

}
