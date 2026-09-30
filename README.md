# EezybotArm MK2 Control

## Kinematic 

### Forward Kinematic

DH Table

|i|$\alpha$|a|d|$\theta$|
|--|--|--|--|--|
|1|$\frac{\pi}{2}$|0|$d_1$|$\theta_1$|
|2|0|$d_2$|0|$\theta_2$|
|3|0|$d_3$|0|$\theta_3$|

General Transformation matrix
$$
A_i=\begin{bmatrix}
c\theta _i & -c\alpha _i s\theta _i & s\alpha _i s\theta _i & a_i c\theta _i \\
s\theta _i & c\alpha _i c\theta _i & -s\alpha _i c\theta _i & a_i s\theta _i \\
0 & s\alpha _i & c\alpha _i & d_i \\
0 & 0 & 0 & 1
\end{bmatrix}
$$
Eezy Transform Matrix
$$
^{0}T_3 = ^{0}T_1.^{1}T_2.^{2}T_3=
\begin{bmatrix}
c\theta_1 & 0 & s\theta_1 & 0 \\
s\theta_1 & 0 & -c\theta_1 & 0 \\
0 & 1 & 0 & d_1 \\
0 & 0 & 0 & 1
\end{bmatrix}
\begin{bmatrix}
c\theta_2 & -s\theta_2 & 0 & d_2c\theta_2 \\
s\theta_2 & c\theta_2 & 0 & d_2s\theta_2 \\
0 & 0 & 1 & 0 \\
0 & 0 & 0 & 1
\end{bmatrix}
\begin{bmatrix}
c\theta_3 & -s\theta_3 & 0 & d_3c\theta_3 \\
s\theta_3 & c\theta_3 & 0 & d_3s\theta_3 \\
0 & 0 & 1 & 0 \\
0 & 0 & 0 & 1
\end{bmatrix}
=
\begin{bmatrix}
c\theta_1c\theta_{23} & -c\theta_1s\theta_{23} & s\theta_1 & c\theta_1(d_2c\theta_2+d_3c\theta_{23} \\
s\theta_1c\theta_{23} & -s\theta_1s\theta_{23} & -c\theta_1 & s\theta_1(d_2c\theta_2+d_3c\theta_{23} \\
s\theta_{23} & c\theta_{23} & 0 & d_1+d_2s\theta_2+d_3s\theta_{23} \\
0 & 0 & 0 & 1
\end{bmatrix}
$$

End-effector coordination:

$$\begin{align*}
X &= c\theta_1(d_2c\theta_2+d_3c\theta_{23}\\[6pt]
Y &= s\theta_1(d_2c\theta_2+d_3c\theta_{23}\\[6pt]
Z &= d_1+d_2s\theta_2+d_3s\theta_{23}
\end{align*}
$$