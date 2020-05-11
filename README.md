# WPF_Nodes
## 基于Windows的便签
### **说明**

点击右上角的 "+" 号添加大任务, 点击大任务的 "+" 号添加对应的子任务, 点击 "-" 删除对应的任务<br >
点击任务前面的 "口" 完成任务, 注意, <font color=#DC143C>完成大任务,会自动将子任务都完成并折叠</font>, 再次点击就取消完成状态<br>
点击左上角的 "Save" 保存信息到本地, 注意, 暂时<font color=#DC143C>没有自动保存</font>, 不保存就写了<br>
点击上方的 "Open" 读取某一个 xml 中的大小任务, 或者将 xml 文件直接拖拽到软件上也可以读取

***
<font color=#DC143C>全局</font>快捷键:<br >
+ Ctrl + S 保存 <br >
+ Enter: 新建一个大任务<br >

<font color=#DC143C>选中一个任务后</font>才能用快捷键: <br >
+ ESC: 展开或折叠一个大任务<br >
+ Tab: 新建一个子任务<br >

<font color="cyan" size=4>
项目开源免费, 求上面点星支持(star ^o^)<br>
有好的意见也可以反馈给我, 比如多久自动保存一次之类待定的需求
</font>

***

### **版本说明**
#### 1.22 [下载点这里](https://github.com/Xnco/WPF_Nodes/tree/master/Build)
* Update: 优化大小任务框的更新逻辑
* Fix: 修复重新展开小任务间隔不相同的问题
* Fix: 修复在多屏情况下定位问题, 防止单双屏切换任务框消失不见, 暂不支持双屏显示

#### 1.21 
* Add: 退出时可以选择是否要保存

#### 1.20 
* Fix: 修复前缀任务的更新太频繁问题

#### 1.19 
* Update: 如果一个任务没前缀, 完成任务会将任务放到最后. 有前缀的话位置不变
* Update: 任务取消完成, 放在未完成的任务之前
* Update: 新增任务不一定添加到最后, 如果存在前缀任务, 则放到最后一个前缀之后, 如果都没前缀, 也放在未完成的任务之前

#### 1.18 
* Update: 调整高度仅限右下角
* Fix: 去掉上方白边

#### 1.17
* Add: 展开大任务的时候箭头旋转
* Add: 多次误操作得到血的教训, 如果大任务没有完成, 在删除的时候需要二次确认才能删除
* Add: 增加图标和右下角Icon, 点击Icon能将窗口置顶
* Update: 手动调整窗口的高度

#### 1.14
* Add: 增加 <font color=#DC143C>打开文件</font> 功能, 能够打开某个 xml 文件并转换成任务
* Add: 添加说明XML, 添加一系列快捷键, 第一次打开才会有
* Add: 根据 *. 给子任务排序的功能<br />
    ><font color="green">eg: "1. xxxx" //就会把这个任务放到最前面的位置</font>

#### 1.10
* Add: 开机启动，主页面加入开机启动开关，<font color=#DC143C>如果更换程序路径要重新开启此功能</font>

#### 1.01
* Add: 直接拖拽适配的 XML 文件到窗口可以直接解析 XML, 对应 XML 格式见 [XML文件夹](https://github.com/Xnco/WPF_Nodes/tree/master/XML)中的模板

#### 1.00
* Update: 更新了一版 UI界面
* Add: 删除功能, 能删除小任务和大任务
* Add: 增加存档功能, 点击Sava按钮能保存所有任务信息到本地, 路径为 <font color=#DC143C size=4>*" \*/我的文档/MyNodes/ "*</font>

#### 0.1 初版
* 仅支持增加大任务和小人物

***

#### 1.0预览图
![Image Text](https://github.com/Xnco/WPF_Nodes/blob/master/Show/Nodes1.0.gif)

#### 初版预览
![Image Text](https://github.com/Xnco/WPF_Nodes/blob/master/Show/MyNodes.gif)
