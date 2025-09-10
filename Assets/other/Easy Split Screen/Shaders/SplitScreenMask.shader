Shader "Mask/SplitScreen" {
	//Simple depthmask shader 
	SubShader {
	    Tags {Queue = Background}
	    Pass {
			ZWrite On
			ColorMask 0}
	}
}