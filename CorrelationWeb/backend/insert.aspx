<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="insert.aspx.cs" Inherits="CorrelationWeb.backend.insert" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <link rel="Stylesheet" type="text/css" href="../res/css/style.css" />
    <link rel="Stylesheet" type="text/css" href="../res/css/datatable.css" />
    <title>随机生成数据</title>
</head>
<body>
    <div id="body_container" class="body_container">
        <div id="title" class="title">
            <h1>随机生成数据</h1>
        </div>
        <div id="container" class="container">
            <form action="" method="get">
            <div id="spec">
                总条数：
                <input type="text" class="filter_input" name="records" />
                <input type="submit" value="提交" class="inputbut"/>
                <fieldset id="multi_values">
                    <legend>多值属性</legend>
                    <div class="dataTables_wrapper">
                        <table>
                            <thead>
                                <tr>
                                    <th>属性</th>
                                    <th>占比（%）</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr class="odd">
                                    <td>火险等级一</td>
                                    <td><input type="text" name="fire_1" class="info_input" /></td>
                                </tr>
                                <tr class="even">
                                    <td>火险等级二</td>
                                    <td><input type="text" name="fire_2" class="info_input" /></td>
                                </tr>
                                <tr class="odd">
                                    <td>火险等级三</td>
                                    <td><input type="text" name="fire_3" class="info_input" /></td>
                                </tr>
                                <tr class="even">
                                    <td>救援速度一</td>
                                    <td><input type="text" name="rescue_1" class="info_input" /></td>
                                </tr>
                                <tr class="odd">
                                    <td>救援速度二</td>
                                    <td><input type="text" name="rescue_2" class="info_input" /></td>
                                </tr>
                                <tr class="even">
                                    <td>救援速度三</td>
                                    <td><input type="text" name="rescue_3" class="info_input" /></td>
                                </tr>
                                <tr class="odd">
                                    <td>救援速度四</td>
                                    <td><input type="text" name="rescue_4" class="info_input" /></td>
                                </tr>
                                <tr class="even">
                                    <td>救援速度五</td>
                                    <td><input type="text" name="rescue_5" class="info_input" /></td>
                                </tr>
                                <tr class="odd">
                                    <td>气温范围一</td>
                                    <td><input type="text" name="temp_1" class="info_input" /></td>
                                </tr>
                                <tr class="even">
                                    <td>气温范围二</td>
                                    <td><input type="text" name="temp_2" class="info_input" /></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </fieldset>
                <fieldset id="bool_value">
                    <legend>布尔属性</legend>
                    <div class="dataTables_wrapper">
                        <table>
                            <thead>
                                <tr>
                                    <th>属性</th>
                                    <th>占比（%）</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr class="odd">
                                    <td>树</td>
                                    <td><input type="text" name="tree" class="info_input" /></td>
                                </tr>
                                <tr class="even">
                                    <td>交通灯</td>
                                    <td><input type="text" name="traffic" class="info_input" /></td>
                                </tr>
                                <tr class="odd">
                                    <td>桥</td>
                                    <td><input type="text" name="bridge" class="info_input" /></td>
                                </tr>
                                <tr class="even">
                                    <td>购物中心</td>
                                    <td><input type="text" name="mall" class="info_input" /></td>
                                </tr>
                                <tr class="odd">
                                    <td>工厂</td>
                                    <td><input type="text" name="factory" class="info_input" /></td>
                                </tr>
                                <tr class="even">
                                    <td>饭店</td>
                                    <td><input type="text" name="restaurant" class="info_input" /></td>
                                </tr>
                                <tr class="odd">
                                    <td>停车场</td>
                                    <td><input type="text" name="parking" class="info_input" /></td>
                                </tr>
                                <tr class="even">
                                    <td>变压器</td>
                                    <td><input type="text" name="transformer" class="info_input" /></td>
                                </tr>
                                <tr class="odd">
                                    <td>医院</td>
                                    <td><input type="text" name="hospital" class="info_input" /></td>
                                </tr>
                                <tr class="even">
                                    <td>学校</td>
                                    <td><input type="text" name="school" class="info_input" /></td>
                                </tr>
                                <tr class="odd">
                                    <td>宾馆</td>
                                    <td><input type="text" name="hotel" class="info_input" /></td>
                                </tr>
                                <tr class="even">
                                    <td>浴室</td>
                                    <td><input type="text" name="bath" class="info_input" /></td>
                                </tr>
                                <tr class="odd">
                                    <td>加油站</td>
                                    <td><input type="text" name="gas_station" class="info_input" /></td>
                                </tr>
                                <tr class="even">
                                    <td>垃圾场</td>
                                    <td><input type="text" name="waste" class="info_input" /></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </fieldset>
            </div>
            </form>
        </div>

        <asp:TextBox ID="txtHello" Width="100px" />

        <div id="footer" class="footer">
            <span>Copyright © 1998 - 2012 <a href="http://www.sjtu.edu.cn">SJTU</a>. All Rights
                Reserved</span>
        </div>
    </div>
</body>
</html>