1 R(v,sourceLeft,sourceRight,targetLeft,targetRight) remap value<br>
2 SAT(v) clamp value<br>
3 L(v0,v1,v) lerp<br>
4 WM = max(wc0,STA(gc-0.5)*wc1*2) 云出现率<br>
5 SRb = SAT(R(ph, 0, 0.07, 0, 1)) 向下映射<br>
6 SRt = SAT(R(ph, wh ×0.2, wh, 1, 0)) 向上映射<br>
7 SA = SRb × SRt <br>
8 DRb = ph ×SAT(R(ph, 0, 0.15, 0, 1)) 向底部降低密度<br>
9 DRt = SAT(R(ph, 0.9, 1.0, 1, 0))) 向顶部的更柔和的过渡降低密度<br>
10 DA = gd × DRb × DRt × wd × 2 密度融合<br>

11 SNsample = R(snr, (sng ×0.625+snb ×0.25+sna ×0.125)−1, 1, 0, 1)  FBM gba <br>  
<br>
scalar probabilities<br>
 global coverage term gc [0,1]<br>
 global density term gd [0,max]<br>
<br>
weather-map<br>
    R(wc0) G(wc1) coverage ,<br>
    B(wh) cloud height<br>
    B(wd) cloud density<br>
<br>
ph[0,1] 当前点云中高度百分比<br>
