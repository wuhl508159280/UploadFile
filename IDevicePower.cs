using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityTool;

namespace ControlTool
{
    /// <summary>
    /// 电源设备接口
    /// </summary>
    public interface IDevicePower
    {
        /// <summary>
        /// 加载电源
        /// </summary>
        /// <param name="deviceType">电源设备类型</param>
        /// <param name="communication">通讯方式</param>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口好</param>
        /// <param name="callBack">是否回调</param>
        void Init(Devices deviceType, Communication communication, string ip, int port, bool callBack = false);
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="deviceType">电源设备类型</param>
        /// <param name="communication">通讯方式</param>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口好</param>
        /// <param name="cmds">下发的byte数据</param>
        void SendCmd(Devices deviceType, Communication communication, string ip, int port, byte[] cmds);
        /// <summary>
        /// 查询是否被连接
        /// </summary>
        /// <param name="ip">连接IP</param>
        /// <returns></returns>
        bool IsOpenConnection(string ip);
        /// <summary>
        /// 指定IP关闭连接
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        bool CloseConnection(string ip);
    }
    /// <summary>
    /// 电源接口实现类
    /// </summary>
    public class ConectDevice : IDevicePower
    {
        public ConectDevice()
        {
      
        }
        public ConectDevice(Action<NewEventArgs> actionExcute)
        {
            action = actionExcute;
        }
        Action<NewEventArgs> action;

        ISocketFather socketFather;

        public bool CloseConnection(string ip)
        {
            socketFather.Close(ip);
            return true;
        }

        public void Init(Devices deviceType, Communication communication, string ip, int port, bool callBack = false)
        {
            switch (deviceType)
            {
                case Devices.N39200:
                    switch (communication)
                    {
                        case Communication.Udp:
                            socketFather = new UDPHelper();

                            var result = socketFather.CreatClient("192.168.0.22", 7001, ip, port);
                            if (callBack)
                            {
                                socketFather.ClientReceiveEvent += Sockt_ClientReceiveEvent;
                            }
                            break;
                        case Communication.Tcp:
                            socketFather = new TCPHelper();
                            socketFather.CreatClient("192.168.0.22", 7001, ip, port);
                            if (callBack)
                            {
                                socketFather.ClientReceiveEvent += Sockt_ClientReceiveEvent;
                            }
                            break;
                        case Communication.Com:
                            break;
                        default:
                            break;
                    }
                    break;
                default: break;
            }
        }

        public bool IsOpenConnection(string ip)
        {
            if (socketFather == null)
            {
                return false;
            }
            return socketFather.GetClientFromServerSocketIPs().First(x => x == ip) != null;
        }

        public void SendCmd(Devices deviceType, Communication communication, string ip, int port, byte[] cmds)
        {
            switch (deviceType)
            {
                case Devices.N39200:
                    switch (communication)
                    {
                        case Communication.Udp:
                            ((UDPHelper)socketFather).ClientSendServer(ip, port, cmds);
                            break;
                        case Communication.Tcp:
                            ((TCPHelper)socketFather).ClientSendServer(ip, port, cmds);
                            break;
                        case Communication.Com:
                            break;
                    }
                    break;
            }

        }

        private void Sockt_ClientReceiveEvent(object sender, NewEventArgs e)
        {
            action?.Invoke(e);
        }
    }

}
